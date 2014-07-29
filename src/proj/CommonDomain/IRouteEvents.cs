namespace CommonDomain
{
	using System;

	public interface IRouteEvents<T> where T : struct
	{
		void Register<TEvent>( Action<TEvent> handler );

		void Register( IAggregate<T> aggregate );
		
		void Dispatch( object eventMessage );
	}
}