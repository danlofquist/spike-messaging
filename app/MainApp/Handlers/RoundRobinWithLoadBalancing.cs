using System;
using System.Collections.Generic;
using System.Linq;

namespace MainApp
{
	public class RoundRobinWithLoadBalancing : IHandleMessage<Message>
	{
		private readonly IHandleMessage<Message>[] _handlers;
		private int _currentIndex;

		public RoundRobinWithLoadBalancing(IEnumerable<IHandleMessage<Message>> handlers)
		{
			_currentIndex = 0;
			_handlers = handlers.ToArray();
		}

		public bool Handle(Message message)
		{
			var start = _currentIndex;
			var handled = false;
			do
			{
				handled = _handlers[_currentIndex++].Handle(message);
				if (_currentIndex >= _handlers.Length)
				{
					_currentIndex = 0;
				}
			} while (!handled && start != _currentIndex);

			return handled;
		}
	}
}

