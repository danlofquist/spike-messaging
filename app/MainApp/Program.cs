using System;

namespace MainApp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var monitor = new Monitor();
			var alertMonitor = new AlertMonitor();

			var bus = new AppBus();
			bus.Subscribe<TempratureChanged>(monitor);
			bus.Subscribe<TempratureChanged>(alertMonitor);
			bus.Subscribe<DecibelChanged>(monitor);

			new TemperatureSensor(bus);
			new SoundSensor(bus);

			Console.ReadKey();

		}
	}

	public class Monitor : 
		IHandleMessage<TempratureChanged>, 
		IHandleMessage<DecibelChanged> 
	{

		public bool Handle (TempratureChanged message)
		{
			Console.Write("T");
			return true;
		}
			
		public bool Handle (DecibelChanged message)
		{
			Console.Write("S");
			return true;
		}
	}

	public class AlertMonitor : IHandleMessage<TempratureChanged> {

		public bool Handle (TempratureChanged message)
		{
			if (message.Temprature > 70) {
				Console.WriteLine ();
				Console.WriteLine("OMG .. RUN !!!!");
				return true;
			}

			return false;
		}
	}

}
