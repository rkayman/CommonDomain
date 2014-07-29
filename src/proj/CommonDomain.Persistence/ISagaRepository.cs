namespace CommonDomain.Persistence
{
	using System;
	using System.Collections.Generic;

	public interface ISagaRepository<in T> where T : struct, IEquatable<T>
	{
		TSaga GetById<TSaga>( T sagaId ) where TSaga : class, ISaga<T>, new();

		void Save<TSaga>( TSaga saga, Guid commitId, Action<IDictionary<string, object>> updateHeaders )
			where TSaga : class, ISaga<T>;
	}
}