using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MainApp
{
	public abstract class Message {
	}

	public interface IHandleMessage<T> where T : Message {
		bool Handle(T message);
	}

	public interface IAppBus {

		void Publish(Message message);
		void Subscribe<T>(IHandleMessage<T> handler) where T : Message;
	}

	public class AppBus : IAppBus
	{
		private readonly ConcurrentDictionary<string, List<IWrapHandler>> _topicHandlers;

		public AppBus()
		{
			_topicHandlers = new ConcurrentDictionary<string, List<IWrapHandler>>();
		}

		public void Subscribe<T>(IHandleMessage<T> handler) where T : Message
		{
			Subscribe(typeof(T).Name, handler);
		}
						
		public void Publish(Message message)
		{
			var messageType = message.GetType();
			do
			{
				Publish(messageType.Name, message);
				messageType = messageType.BaseType;
			} while (messageType != typeof(object));
		}
			
		private void Publish(string topic, Message message)
		{
			List<IWrapHandler> handlers;
			if (_topicHandlers.TryGetValue(topic, out handlers))
			{
				foreach (var handler in handlers)
				{
					handler.Handle(message);
				}
			}
		}

		private void Subscribe<T>(string topic, IHandleMessage<T> handler) where T : Message
		{
			var narrowingHandler = new MessageHandler<T>(handler);
			_topicHandlers.AddOrUpdate(
				topic,
				x => new List<IWrapHandler>() { narrowingHandler },
				(s, list) => new List<IWrapHandler>(list) { narrowingHandler });
		}
	}

	interface IWrapHandler : IHandleMessage<Message> {
		bool IsSame(object other);
	}


	internal class MessageHandler<T> : NarrowingHandler<Message, T>, IWrapHandler where T : Message
	{
		private readonly IHandleMessage<T> _handle;

		public MessageHandler(IHandleMessage<T> handle)
			: base(handle)
		{
			_handle = handle;
		}

		public bool IsSame(object other)
		{
			return ReferenceEquals(_handle, other);
		}
	}

	internal class NarrowingHandler<TBase, TDerived> : IHandleMessage<TBase>
		where TDerived : class, TBase
		where TBase : Message
	{
		private readonly IHandleMessage<TDerived> _handle;

		public NarrowingHandler(IHandleMessage<TDerived> handle)
		{
			_handle = handle;
		}

		public bool Handle(TBase message)
		{
			var msg = message as TDerived;
			if (msg != null)
			{
				return _handle.Handle(msg);
			}
			return false;
		}
	}
	internal class WideningHandler<TBase, TDerived> : IHandleMessage<TDerived> where TDerived:TBase where TBase:Message
	{
		private readonly IHandleMessage<TBase> _handler;

		public WideningHandler(IHandleMessage<TBase> handler)
		{
			_handler = handler;
		}

		public bool Handle(TDerived message)
		{
			return _handler.Handle(message);
		}
	}
}
