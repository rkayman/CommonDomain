namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;

	public class RegistrationMessageRouter<T> : IRouteMessages<T> where T : struct
	{
		private readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
		private IAggregate<T> registered;

		public virtual void Register<TMessage>(Action<TMessage> handler)
		{
			handlers[typeof(TMessage)] = message => handler((TMessage) message);
		}

		public virtual void Register(IAggregate<T> aggregate)
		{
			if (aggregate == null)
				throw new ArgumentNullException("aggregate");

			registered = aggregate;
		}

		public virtual void Dispatch(object message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			Action<object> handler;

			if (handlers.TryGetValue(message.GetType(), out handler))
				handler(message);

			registered.ThrowHandlerNotFound(message);
		}
	}
}