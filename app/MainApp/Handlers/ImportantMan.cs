using System;
using System.Collections.Generic;

namespace MainApp
{
		
	public class ImportantMan : IHandleMessage<NeedToTakeAction> {

		private readonly IAppBus _bus;

		public ImportantMan(IAppBus bus)
		{
			_bus = bus;
		}

		public bool Handle (NeedToTakeAction message)
		{
			var sensor = message.MessageWithIssues as TemperatureChanged;
			if (sensor != null) {
				Console.WriteLine ("Stopping sensor : {0} for 2 seconds", sensor.SensorName);
				_bus.Publish(new DisableSensor(sensor.SensorName));
				_bus.Publish(new SendMessageLater(DateTime.UtcNow.AddSeconds(2), new EnableSensor (sensor.SensorName)));
			} else {
				Console.WriteLine ("Sending mail to an even more important man, problem with : {0}", message.MessageWithIssues.GetType().Name);
			}

			return true;
		}
	}
	
}
