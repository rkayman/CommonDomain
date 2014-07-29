namespace CommonDomain.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public abstract class AggregateBase<T> : IAggregate<T>, IEquatable<IAggregate<T>> where T : struct
	{
		private readonly ICollection<object> uncommittedEvents = new LinkedList<object>();

		private IRouteEvents<T> registeredRoutes;

		protected AggregateBase() : this( null ) { }

		protected AggregateBase( IRouteEvents<T> handler )
		{
			if (handler == null)
				return;

			RegisteredRoutes = handler;
			RegisteredRoutes.Register( this );
		}

		public T Id { get; protected set; }
		public int Version { get; protected set; }

		protected IRouteEvents<T> RegisteredRoutes
		{
			get
			{
				return registeredRoutes ?? (registeredRoutes = new ConventionEventRouter<T>( true, this ));
			}
			set
			{
				if (value == null)
					throw new InvalidOperationException( "AggregateBase must have an event router to function" );

				registeredRoutes = value;
			}
		}

		protected void Register<TEvent>( Action<TEvent> route )
		{
			RegisteredRoutes.Register( route );
		}

		protected void RaiseEvent( object @event )
		{
			((IAggregate<T>)this).ApplyEvent( @event );
			uncommittedEvents.Add( @event );
		}

		void IAggregate<T>.ApplyEvent( object @event )
		{
			RegisteredRoutes.Dispatch( @event );
			Version++;
		}

		ICollection IAggregate<T>.GetUncommittedEvents()
		{
			return (ICollection)uncommittedEvents;
		}

		void IAggregate<T>.ClearUncommittedEvents()
		{
			uncommittedEvents.Clear();
		}

		IMemento<T> IAggregate<T>.GetSnapshot()
		{
			var snapshot = GetSnapshot();
			snapshot.Id = Id;
			snapshot.Version = @Version;
			return snapshot;
		}

		protected virtual IMemento<T> GetSnapshot()
		{
			return null;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			return Equals( obj as IAggregate<T> );
		}

		public virtual bool Equals( IAggregate<T> other )
		{
			return null != other && Id.Equals( other.Id );
		}
	}
}