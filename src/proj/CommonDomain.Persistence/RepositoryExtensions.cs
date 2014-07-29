namespace CommonDomain.Persistence
{
	using System;

	public static class RepositoryExtensions
	{
		public static void Save<T>( this IRepository<T> repository, IAggregate<T> aggregate, Guid commitId )
			where T : struct
		{
			repository.Save( aggregate, commitId, a => { } );
		}
	}
}