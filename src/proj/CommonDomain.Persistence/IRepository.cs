namespace CommonDomain.Persistence
{
	using System;
	using System.Collections.Generic;

	public interface IRepository<T> where T : struct
	{
		TAggregate GetById<TAggregate>( T id ) where TAggregate : class, IAggregate<T>;

		TAggregate GetById<TAggregate>( T id, int version ) where TAggregate : class, IAggregate<T>;

		void Save<TAggregate>( TAggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders )
			where TAggregate : class, IAggregate<T>;
	}
}