using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{
		
	public class Monitor : IHandleMessage<TemperatureChanged>
	{
		private readonly Random _random;
		public Monitor ()
		{
			_random = new Random();
		}
		public bool Handle (TemperatureChanged message)
		{
			//Console.WriteLine(message.SensorName + " reading : " + message.Value.ToString("N1"));
			Thread.Sleep(_random.Next(1,300));
			return true;
		}
	}
	
}
	