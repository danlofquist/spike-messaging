using System;

namespace MainApp
{
	public class TimeToLiveHandler : IHandleMessage<Message>
	{
		private readonly IHandleMessage<Message> _handler;

		public TimeToLiveHandler(IHandleMessage<Message> handler)
		{
			_handler = handler;
		}

		public bool Handle(Message message)
		{
			var ttl = message as IHaveTimeToLive;
			if (ttl != null) 
			{
				if (ttl.TimeToLive > DateTime.UtcNow) 				
				{
					return _handler.Handle (message);
				} 
				else 
				{
					Console.WriteLine("Dropping message due to ttl");
				}
			}

			return true;
		}
	}
}

