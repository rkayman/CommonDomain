namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public sealed class ConventionEventRouter : IRouteEvents
	{
	    readonly bool throwOnApplyNotFound;
	    private readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
		private IAggregate registered;

	    public ConventionEventRouter() : this(true)
	    {
	    }

	    public ConventionEventRouter(bool throwOnApplyNotFound)
	    {
	        this.throwOnApplyNotFound = throwOnApplyNotFound;
	    }

        public ConventionEventRouter(bool throwOnApplyNotFound, IAggregate aggregate) : this(throwOnApplyNotFound)
	    {
	        Register(aggregate);
	    }

		public void Register<T>(Action<T> handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");

			Register(typeof(T), @event => handler((T)@event));
		}

		public void Register(IAggregate aggregate)
		{
			if (aggregate == null)
				throw new ArgumentNullException("aggregate");

			registered = aggregate;

			// Get instance methods named Apply with one parameter returning void
			var applyMethods = aggregate.GetType()
			                            .GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
			                            .Where( m =>
				                                    m != null &&
				                                    (m.Name == "Apply" && m.GetParameters().Length == 1 &&
				                                     (m.ReturnParameter != null &&
				                                      m.ReturnParameter.ParameterType == typeof (void))) )
			                            .Select( m => new
			                            {
				                            Method = m,
				                            MessageType = m.GetParameters().Single().ParameterType
			                            } );

			foreach (var apply in applyMethods)
			{
				var applyMethod = apply.Method;
				handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] { m }));
			}
		}

		public void Dispatch(object eventMessage)
		{
			if (eventMessage == null)
				throw new ArgumentNullException("eventMessage");

			Action<object> handler;
            if (handlers.TryGetValue(eventMessage.GetType(), out handler))
				handler(eventMessage);
            else if(throwOnApplyNotFound)
				registered.ThrowHandlerNotFound(eventMessage);
		}

		private void Register(Type messageType, Action<object> handler)
		{
			handlers[messageType] = handler;
		}
	}
}