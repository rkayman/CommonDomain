namespace CommonDomain.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public abstract class AggregateBase<T> : IAggregate<T>, IEquatable<IAggregate<T>> where T : struct
	{
		private readonly ICollection<object> uncommittedEvents = new LinkedList<object>();

		private IRouteMessages<T> registeredEvents;
		private IRouteMessages<T> registeredCommands; 

		protected AggregateBase() : this( null, null ) { }

		protected AggregateBase(IRouteMessages<T> events, IRouteMessages<T> commands)
		{
			RegisterEvents(events);
			RegisterCommands(commands);
		}

		private void RegisterEvents(IRouteMessages<T> events)
		{
			if (events == null) return;

			RegisteredEvents = events;
			RegisteredEvents.Register(this);
		}

		private void RegisterCommands(IRouteMessages<T> commands)
		{
			if (commands == null) return;

			RegisteredCommands = commands;
			RegisteredCommands.Register(this);
		}

		public T Id { get; protected set; }
		public int Version { get; protected set; }

		protected IRouteMessages<T> RegisteredEvents
		{
			get { return registeredEvents ?? (registeredEvents = new ConventionMessageRouter<T>(true, this)); }
			set
			{
				if (value == null)
					throw new InvalidOperationException( "AggregateBase must have an event router to function" );

				registeredEvents = value;
			}
		}

		protected IRouteMessages<T> RegisteredCommands
		{
			get { return registeredCommands ?? (registeredCommands = new ConventionMessageRouter<T>(true, this)); }
			set
			{
				if (value == null)
					throw new InvalidOperationException("AggregateBase must have a command router to function");

				registeredCommands = value;
			}
		}

		protected void RegisterEvent<TMessage>(Action<TMessage> route)
		{
			RegisteredEvents.Register(route);
		}

		protected void RaiseEvent( object @event )
		{
			((IAggregate<T>)this).ApplyEvent( @event );
			uncommittedEvents.Add( @event );
		}

		void IAggregate<T>.ApplyEvent( object @event )
		{
			RegisteredEvents.Dispatch( @event );
			@Version++;
		}

		protected void RegisterCommand<TMessage>(Action<TMessage> route)
		{
			RegisteredCommands.Register(route);
		}

		public virtual void DoCommand(object command)
		{
			RegisteredCommands.Dispatch(command);
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