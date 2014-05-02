using System;
using System.Collections.Generic;

namespace MainApp
{
	public class HighTemperatureThreshold : IThreshold {
		private readonly decimal _threshold;

		public HighTemperatureThreshold(decimal threshold)
		{
			_threshold = threshold;			
		}
			
		public bool IsWithinThreshold (decimal value)
		{
			return value < _threshold ? true : false;
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			var bus = new AppBus();
			var monitor = new Monitor();
			var importantMan = new ImportantMan(bus);
			var alarmClock = new AlarmClock(bus);

			bus.Subscribe(alarmClock);
			bus.Subscribe(importantMan);
			bus.Subscribe(new AlertMonitor(new NarrowingHandler<Message, TemperatureChanged>(monitor), new HighTemperatureThreshold(120), bus));


			var startables = new List<IStartable> ();
			startables.Add(alarmClock);

			for ( var ii = 0; ii < 50; ii++ ) {
				startables.Add(new TemperatureSensor(bus, ii.ToString()));
			}
				
			startables.ForEach(s => s.Start());

			Console.ReadKey();
		}
	}
		

	public class Monitor : IHandleMessage<TemperatureChanged>
	{
		public bool Handle (TemperatureChanged message)
		{
			Console.WriteLine(message.SensorName + " reading : " + message.Value.ToString("N1"));
			return true;
		}
	}

	public class AlertMonitor : IHandleMessage<Message> {

		private readonly IHandleMessage<Message> _handler;
		private readonly IAppBus _bus;
		private readonly IThreshold _threshold;

		public AlertMonitor(IHandleMessage<Message> handler, IThreshold threshold, IAppBus bus)
		{
			_bus = bus;
			_threshold = threshold;
			_handler = handler;			
		}

		public bool Handle(Message message)
		{
			var alertableMessage = message as IAlertable;
			if ( alertableMessage != null && !_threshold.IsWithinThreshold (alertableMessage.Value)) {
				_bus.Publish(new NeedToTakeAction(message));
			}

			return _handler.Handle(message);
		}
	}
}
