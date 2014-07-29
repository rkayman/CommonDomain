namespace CommonDomain.Persistence
{
	using System;

	public interface IConstructAggregates<T> where T : struct
	{
		IAggregate<T> Build( Type type, T id, IMemento<T> snapshot );
	}
}