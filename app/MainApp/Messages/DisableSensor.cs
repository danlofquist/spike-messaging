using System;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{
	public class DisableSensor : Message {
		public string SensorToDisable { get; private set; }

		public DisableSensor(string sensorName)
		{
			SensorToDisable = sensorName;
		}
	}
	
}
