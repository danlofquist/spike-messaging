using System;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{
	public class DecibelChanged : Message {

		public decimal Decibel { get; private set;}

		public DecibelChanged(decimal decibel)
		{
			Decibel = decibel;			
		}
	}

	public class SoundSensor
	{
		private readonly IAppBus _appBus;

		public SoundSensor(IAppBus appBus)
		{
			_appBus = appBus;
			StartSensor();
		}

		void StartSensor ()
		{
			Task.Run( () => {
				var r = new Random();
				while (true) {
					var t = r.Next(0, 10);
					_appBus.Publish(new DecibelChanged(t));
					var nextSleep = r.Next(100,500);
					Thread.Sleep(nextSleep);
				}
			});
		}
	}
}
