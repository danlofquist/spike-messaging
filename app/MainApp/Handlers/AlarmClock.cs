using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace MainApp
{
	public class AlarmClock : IHandleMessage<SendMessageLater>, IStartable
	{
		private readonly ConcurrentQueue<SendMessageLater> _alarms;
		private volatile bool _stop;
		private Thread _thread;
		private readonly AutoResetEvent _stopped;
		private readonly IAppBus _bus;

		public AlarmClock(IAppBus bus)
		{
			_bus = bus;
			_alarms = new ConcurrentQueue<SendMessageLater>();
			_stopped = new AutoResetEvent(false);
		}

		public bool Handle(SendMessageLater message)
		{
			_alarms.Enqueue(message);
			return true;
		}

		public void Start()
		{
			_thread = new Thread(Process);
			_stop = false;
			_thread.Start();
		}

		public void Stop()
		{
			_stop = true;
			_stopped.WaitOne(5000);
		}

		private void Process()
		{
			while (!_stop)
			{
				SendMessageLater message;
				SendMessageLater marker = null;

				while (_alarms.TryDequeue(out message))
				{
					if (message == marker)
					{
						_alarms.Enqueue(message);
						break;
					}

					if ( (DateTime.UtcNow - message.When).Seconds > 0  )
					{
						_bus.Publish(message.Message);
					}
					else
					{
						marker = marker ?? message;
						_alarms.Enqueue(message);
					}
				}

				Thread.Sleep(1);
			}
			_stopped.Set();
		}
	}

}

