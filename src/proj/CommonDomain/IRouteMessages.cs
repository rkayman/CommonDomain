namespace CommonDomain
{
	using System;

	public interface IRouteMessages<T> where T : struct
	{
		void Register<TMessage>(Action<TMessage> handler);

		void Register(IAggregate<T> aggregate);

		void Dispatch(object message);
	}
}