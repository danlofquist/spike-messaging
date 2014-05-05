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

}
	