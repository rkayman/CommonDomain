namespace CommonDomain
{
	using System.Collections;

	public interface IAggregate<T> where T : struct
	{
		T Id { get; }
		int Version { get; }

		void ApplyEvent( object @event );

		ICollection GetUncommittedEvents();

		void ClearUncommittedEvents();

		IMemento<T> GetSnapshot();
	}
}