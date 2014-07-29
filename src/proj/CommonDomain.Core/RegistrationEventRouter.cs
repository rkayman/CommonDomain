namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;

	public class RegistrationEventRouter<T> : IRouteEvents<T>
		where T : struct
	{
		private readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
		private IAggregate<T> registered;

		public virtual void Register<TEvent>( Action<TEvent> handler )
		{
			handlers[typeof( TEvent )] = @event => handler( (TEvent)@event );
		}

		public virtual void Register( IAggregate<T> aggregate )
		{
			if (aggregate == null)
				throw new ArgumentNullException( "aggregate" );

			registered = aggregate;
		}

		public virtual void Dispatch( object eventMessage )
		{
			if (eventMessage == null)
				throw new ArgumentNullException( "eventMessage" );

			Action<object> handler;

			if (handlers.TryGetValue( eventMessage.GetType(), out handler ))
				handler( eventMessage );

			registered.ThrowHandlerNotFound( eventMessage );
		}
	}
}