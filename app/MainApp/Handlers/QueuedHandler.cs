using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MainApp
{
	public class QueuedHandler : IHandleMessage<Message>, IStartable, IProduceStatistics
	{
		public string Name { get { return _name; } }

		private readonly IHandleMessage<Message> _handler;
		private readonly string _name;
		private readonly int _maxCount;
		private readonly ConcurrentQueue<Message> _messageQueue;
		private volatile bool _stop;
		private Thread _thread;
		private readonly AutoResetEvent _stopped;
		private int _processed;

		public QueuedHandler(IHandleMessage<Message> handler, string name, int maxCount = 0)
		{
			_handler = handler;
			_name = name;
			_maxCount = maxCount;
			_messageQueue = new ConcurrentQueue<Message>();
			_stopped = new AutoResetEvent(false);
		}

		public void Start()
		{
			_thread = new Thread(Process) { Name = Name };
			_stop = false;
			_thread.Start();
		}

		private void Process()
		{
			while (!_stop)
			{
				Message message;
				while (_messageQueue.TryDequeue(out message))
				{
					_handler.Handle(message);
					_processed++;
				}
				Thread.Sleep(1);
			}
			_stopped.Set();
		}

		public void Stop()
		{
			_stop = true;
			_stopped.WaitOne(5000);
		}

		public bool Handle(Message message)
		{
			if ((_maxCount == 0) || (_maxCount != 0 && _messageQueue.Count < _maxCount))
			{
				_messageQueue.Enqueue(message);
				return true;

			}
			else
			{
				return false;
			}
		}

		public Statistics GetStatistics()
		{
			var count = _messageQueue.Count;
			return new Statistics (_name, count, _processed);
		}

	}
}

