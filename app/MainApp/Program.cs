using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

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
			var alarmClock = new AlarmClock(bus);
			var startables = new List<IStartable> ();

			var importantManQueue = new QueuedHandler( new NarrowingHandler<Message, NeedToTakeAction>( new ImportantMan(bus)), "Important man");																														 
			var monitorQueue = new QueuedHandler (
				                   new RoundRobinWithLoadBalancing(
										new[] {
											CreateMonitor(bus, startables, "Monitor q #01"),
											CreateMonitor(bus, startables, "Monitor q #02"),
											CreateMonitor(bus, startables, "Monitor q #03")
										}
									), "Monitor queues"
								);

			bus.Subscribe(alarmClock);
			bus.Subscribe(new WideningHandler<Message,NeedToTakeAction>(importantManQueue));
			bus.Subscribe(monitorQueue);

			startables.Add(alarmClock);
			startables.Add(monitorQueue);
			startables.Add(importantManQueue);

			startables.ForEach(s => s.Start());

			Task.Run (() => {
				while (true)
				{
					Thread.Sleep(1000);
					startables.ForEach(x => {
						var stat = x as IProduceStatistics;
						if ( stat != null ) {
							Console.WriteLine(stat.GetStatistics());
						}
					});
				}
			});

			for ( var ii = 0; ii < 50; ii++ ) {
				new TemperatureSensor (bus, ii.ToString ()).Start();
			}

			Console.ReadKey();
		}

		private static QueuedHandler CreateMonitor(IAppBus bus,List<IStartable> startables, string monitorName) {
			var m = new QueuedHandler (new AlertMonitor (new NarrowingHandler<Message, TemperatureChanged> (new Monitor()), new HighTemperatureThreshold (120), bus), monitorName);
			startables.Add(m);
			return m;
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
	