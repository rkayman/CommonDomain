namespace CommonDomain
{
	using System;
	using System.Collections;

	public interface IAggregate
	{
		long Id { get; }
		Guid Guid { get; }
		int Version { get; }

		void ApplyEvent(object @event);
	
		ICollection GetUncommittedEvents();		
		void ClearUncommittedEvents();

		IMemento GetSnapshot();
	}
}