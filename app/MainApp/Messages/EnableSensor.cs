using System;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{

	public class EnableSensor : Message {
		public string SensorToEnable { get; private set; }

		public EnableSensor(string sensorName)
		{
			SensorToEnable = sensorName;
		}
	}
	
}
