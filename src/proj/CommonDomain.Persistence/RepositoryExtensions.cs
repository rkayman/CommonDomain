using System;

namespace CommonDomain.Persistence
{
    public static class RepositoryExtensions
    {
         public static void Save(this IRepository repository, IAggregate aggregate, Guid commitId)
         {
	         repository.Save( aggregate, commitId, a => { } );
         }

	    public static void Save( this IRepository repository, IAggregate aggregate, long commitId )
	    {
		    repository.Save( aggregate, commitId, a => { } );
	    }
    }
}