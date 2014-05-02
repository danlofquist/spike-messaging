using System;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{
	public class TemperatureSensor : 
		IHandleMessage<DisableSensor>, 
		IHandleMessage<EnableSensor>, 
		IStartable
	{
		private readonly IAppBus _appBus;
		private readonly string _name;
		private volatile bool _stop;

		public TemperatureSensor(IAppBus appBus, string name)
		{
			_name = name;
			_appBus = appBus;

			_appBus.Subscribe<DisableSensor>(this);
			_appBus.Subscribe<EnableSensor>(this);

			MakeSensorTick();
			StartSensor();
		}
			
		public bool Handle(DisableSensor message)
		{
			if (message.SensorToDisable == this.ToString ())
				StopSensor();

			return true;
		}
			
		public bool Handle(EnableSensor message)
		{
			if (message.SensorToEnable == this.ToString ())
				StartSensor();

			return true;
		}

		private void StopSensor() 
		{
			_stop = true;
		}

		private void StartSensor()
		{
			_stop = false;
		}

		private void MakeSensorTick() 
		{
			Task.Run( () => {
				var r = new Random();
				while (true) 
				{
					if (!_stop) {
						_appBus.Publish(new TemperatureChanged(r.Next(0, 150),this.ToString()));
						Thread.Sleep(50);
					}
					Thread.Sleep(1);
				}
			});
		}

		public void Start()
		{
			StartSensor();
		}

		public override string ToString ()
		{
			return string.Format ("[TemperatureSensor] {0}", _name);
		}

	}
}
