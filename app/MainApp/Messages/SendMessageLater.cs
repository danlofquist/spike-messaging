using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace MainApp
{
	public class SendMessageLater : Message
	{
		public DateTime When { get; private set;}
		public Message Message { get; private set; }

		public SendMessageLater(DateTime when, Message message)
		{
			When = when;
			Message = message;
		}
	}
}
