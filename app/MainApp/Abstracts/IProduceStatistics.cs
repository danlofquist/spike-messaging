using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MainApp
{
	public interface IProduceStatistics
	{
		Statistics GetStatistics();
	}
}

