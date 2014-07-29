namespace CommonDomain
{
	using System;
	using System.Collections;

	public interface ISaga<out T> where T : struct, IEquatable<T>
	{
		T Id { get; }
		int Version { get; }

		void Transition( object message );

		ICollection GetUncommittedEvents();
		void ClearUncommittedEvents();

		ICollection GetUndispatchedMessages();
		void ClearUndispatchedMessages();
	}
}