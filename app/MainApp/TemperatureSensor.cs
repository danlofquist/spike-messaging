using System;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{
	public class TempratureChanged : Message {

		public decimal Temprature { get; private set;}

		public TempratureChanged(decimal temprature)
		{
			Temprature = temprature;			
		}
	}

	public class TemperatureSensor
	{
		private readonly IAppBus _appBus;

		public TemperatureSensor(IAppBus appBus)
		{
			_appBus = appBus;
			StartSensor();
		}

		void StartSensor ()
		{
			Task.Run( () => {
				var r = new Random();
				while (true) {
					var t = r.Next(0, 90);
					_appBus.Publish(new TempratureChanged(t));
					var nextSleep = r.Next(100,500);
					Thread.Sleep(nextSleep);
				}
			});
		}
	}
}
