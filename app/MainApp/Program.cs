using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{
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

			RunStatistics(startables);
			RunFakeSensors(bus);

			Console.ReadKey();
		}

		static void RunStatistics(List<IStartable> startables)
		{
			Task.Factory.StartNew (() =>  {
				while (true) {
					Thread.Sleep (1000);
					startables.ForEach (x =>  {
						var stat = x as IProduceStatistics;
						if (stat != null) {
							Console.WriteLine (stat.GetStatistics ());
						}
					});
				}
			});
		}

		private static QueuedHandler CreateMonitor(IAppBus bus,List<IStartable> startables, string monitorName) {
			var queueHandler = new QueuedHandler(
						new TimeToLiveHandler(
							new AlertMonitor (
								new NarrowingHandler<Message, TemperatureChanged> (new Monitor()), new HighTemperatureThreshold (120), bus)
							), monitorName);

			startables.Add(queueHandler);
			return queueHandler;
		}

		static void RunFakeSensors(AppBus bus)
		{
			for (var ii = 0; ii < 10; ii++) {
				new TemperatureSensor(bus, ii.ToString()).Start();
			}
		}
	}
		

}
	