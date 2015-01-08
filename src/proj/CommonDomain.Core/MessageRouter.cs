namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;

	public class MessageRouter<TMessage, TResult>
		where TMessage : class
		where TResult : class
	{
		private readonly Dictionary<Type, Func<TMessage, TResult>> handlers =
			new Dictionary<Type, Func<TMessage, TResult>>();

		public void Register(Func<TMessage, TResult> handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");

			handlers[typeof(TMessage)] = handler;
		}

		public void Register(Action<TMessage> handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");

			handlers[typeof(TMessage)] = message =>
			{
				handler(message);
				return Nothing.Value as TResult;
			};
		}

		public TResult Dispatch(TMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");

			Func<TMessage, TResult> handler;

			if (handlers.TryGetValue(message.GetType(), out handler)) return handler(message);
			
			var exceptionMessage = "Aggregate of type '{0}' raised a message of type '{1}' but no handler could be found to handle the message."
				.FormatWith(typeof(TResult).Name, message.GetType().Name);

			throw new HandlerForDomainEventNotFoundException(exceptionMessage);
		}
	}
}