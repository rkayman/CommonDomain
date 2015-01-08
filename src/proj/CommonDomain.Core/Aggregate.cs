namespace CommonDomain.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class Aggregate : IAggregate<long>, IEquatable<IAggregate<long>>
	{
		private readonly ICollection<object> uncommittedEvents = new LinkedList<object>();

		private MessageRouter<object,Aggregate> registeredMessages;

		protected Aggregate() { }

		public long Id { get; protected set; }
		public int Version { get; protected set; }

		protected MessageRouter<object,Aggregate> RegisteredMessages
		{
			get { return registeredMessages ?? (registeredMessages = new MessageRouter<object, Aggregate>()); }
			set
			{
				if (value == null)
					throw new InvalidOperationException( "Aggregate must have a message router to function" );

				registeredMessages = value;
			}
		}

		protected void Register(Func<object, Aggregate> handler)
		{
			registeredMessages.Register(handler);
		}

		public Aggregate Execute(object command)
		{
			return registeredMessages.Dispatch(command);
		}

		protected void Register( Action<object> handler )
		{
			RegisteredMessages.Register( handler );
		}

		protected void RaiseEvent( object @event )
		{
			((IAggregate<long>)this).ApplyEvent( @event );
			uncommittedEvents.Add( @event );
		}

		void IAggregate<long>.ApplyEvent( object @event )
		{
			RegisteredMessages.Dispatch( @event );
			@Version++;
		}

		ICollection IAggregate<long>.GetUncommittedEvents()
		{
			return (ICollection)uncommittedEvents;
		}

		void IAggregate<long>.ClearUncommittedEvents()
		{
			uncommittedEvents.Clear();
		}

		IMemento<long> IAggregate<long>.GetSnapshot()
		{
			var snapshot = GetSnapshot();
			snapshot.Id = Id;
			snapshot.Version = @Version;
			return snapshot;
		}

		protected virtual IMemento<long> GetSnapshot()
		{
			return null;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			return Equals( obj as IAggregate<long> );
		}

		public virtual bool Equals( IAggregate<long> other )
		{
			return null != other && Id.Equals( other.Id );
		}
	}
}