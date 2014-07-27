namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// The conflict detector is used to determine if the events to be committed represent
	/// a true business conflict as compared to events that have already been committed, thus
	/// allowing reconciliation of optimistic concurrency problems.
	/// </summary>
	/// <remarks>
	/// The implementation contains some internal lambda "magic" which allows casting between
	/// TCommitted, TUncommitted, and System.Object and in a completely type-safe way.
	/// </remarks>
	public class ConflictDetector : IDetectConflicts
	{
		private readonly IDictionary<Type, IDictionary<Type, ConflictDelegate>> actions =
			new Dictionary<Type, IDictionary<Type, ConflictDelegate>>();

		public void Register<TUncommitted, TCommitted>(ConflictDelegate handler)
			where TUncommitted : class
			where TCommitted : class
		{
			IDictionary<Type, ConflictDelegate> inner;
			if (!actions.TryGetValue(typeof(TUncommitted), out inner))
				actions[typeof(TUncommitted)] = inner = new Dictionary<Type, ConflictDelegate>();

			inner[typeof(TCommitted)] = (uncommitted, committed) =>
				handler(uncommitted as TUncommitted, committed as TCommitted);
		}

		public bool ConflictsWith(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents)
		{
			if (uncommittedEvents == null) throw new ArgumentNullException( "uncommittedEvents" );
			if (committedEvents == null) throw new ArgumentNullException( "committedEvents" );

			var uncommittedEventArray = uncommittedEvents as object[] ?? uncommittedEvents.ToArray();
			var committedEventArray = committedEvents as object[] ?? committedEvents.ToArray();

			return (from object uncommitted in uncommittedEventArray
					from object committed in committedEventArray
					where Conflicts(uncommitted, committed)
					select uncommittedEventArray).Any();
		}

		private bool Conflicts(object uncommitted, object committed)
		{
			IDictionary<Type, ConflictDelegate> registration;
			if (!actions.TryGetValue(uncommitted.GetType(), out registration))
				return uncommitted.GetType() == committed.GetType(); // no reg, only conflict if the events are the same time

			ConflictDelegate callback;
			if (!registration.TryGetValue(committed.GetType(), out callback))
				return true;

			return callback(uncommitted, committed);
		}
	}
}