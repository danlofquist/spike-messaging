using System;
using System.Collections.Generic;

namespace MainApp
{
	public class NeedToTakeAction : Message
	{
		public Message MessageWithIssues { get; private set; }

		public NeedToTakeAction (Message messageWithIssues)
		{
			MessageWithIssues = messageWithIssues;			
		}

	}
}
