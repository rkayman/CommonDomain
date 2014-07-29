namespace CommonDomain.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class SagaBase<TMessage, TId> : ISaga<TId>, IEquatable<ISaga<TId>>
		where TMessage : class
		where TId : struct, IEquatable<TId>
	{
		private readonly IDictionary<Type, Action<TMessage>> handlers = new Dictionary<Type, Action<TMessage>>();
		private readonly ICollection<TMessage> uncommitted = new LinkedList<TMessage>();
		private readonly ICollection<TMessage> undispatched = new LinkedList<TMessage>();

		public TId Id { get; protected set; }
		public int Version { get; private set; }

		protected void Register<TRegisteredMessage>( Action<TRegisteredMessage> handler )
			where TRegisteredMessage : class, TMessage
		{
			handlers[typeof (TRegisteredMessage)] = message => handler( message as TRegisteredMessage );
		}

		public void Transition( object message )
		{
			handlers[message.GetType()]( message as TMessage );
			uncommitted.Add( message as TMessage );
			Version++;
		}

		ICollection ISaga<TId>.GetUncommittedEvents()
		{
			return uncommitted as ICollection;
		}

		void ISaga<TId>.ClearUncommittedEvents()
		{
			uncommitted.Clear();
		}

		protected void Dispatch( TMessage message )
		{
			undispatched.Add( message );
		}

		ICollection ISaga<TId>.GetUndispatchedMessages()
		{
			return undispatched as ICollection;
		}

		void ISaga<TId>.ClearUndispatchedMessages()
		{
			undispatched.Clear();
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals( object obj )
		{
			return Equals( obj as ISaga<TId> );
		}

		public virtual bool Equals( ISaga<TId> other )
		{
			return null != other && Id.Equals( other.Id );
		}
	}
}