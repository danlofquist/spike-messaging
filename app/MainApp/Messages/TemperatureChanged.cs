using System;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{

	public class TemperatureChanged : Message, IAlertable, IHaveTimeToLive {

		public decimal Value { get; private set;}
		public string SensorName { get; private set; }
		public DateTime TimeToLive { get; private set; }

		public TemperatureChanged(decimal temprature, string sensorName)
		{
			Value = temprature;
			SensorName = sensorName;
			TimeToLive = DateTime.UtcNow.AddMilliseconds(300);
		}
	}
	
}
