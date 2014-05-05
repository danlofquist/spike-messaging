using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MainApp
{

	public class AlertMonitor : IHandleMessage<Message> {

		private readonly IHandleMessage<Message> _handler;
		private readonly IAppBus _bus;
		private readonly IThreshold _threshold;

		public AlertMonitor(IHandleMessage<Message> handler, IThreshold threshold, IAppBus bus)
		{
			_bus = bus;
			_threshold = threshold;
			_handler = handler;			
		}

		public bool Handle(Message message)
		{
			var alertableMessage = message as IAlertable;
			if ( alertableMessage != null && !_threshold.IsWithinThreshold (alertableMessage.Value)) {
				_bus.Publish(new NeedToTakeAction(message));
			}

			return _handler.Handle(message);
		}
	}
}
	