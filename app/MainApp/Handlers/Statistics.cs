using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MainApp
{
	public class Statistics
	{
		public string QueueName { get; private set; }
		public long InQueue { get; private set; }
		public long Processed { get; private set; }

		public Statistics(string queueName, long inQueue, long processed)
		{
			QueueName = queueName;
			InQueue = inQueue;
			Processed = processed;
		}

		public override string ToString()
		{
			return string.Format("Queue {0} : Processed {1} : InQueue {2}", QueueName, Processed, InQueue);
		}

	}
}

