using System;
using System.Collections.Generic;

namespace MainApp
{
	public interface IThreshold {
		bool IsWithinThreshold(decimal value);
	}
			
}
