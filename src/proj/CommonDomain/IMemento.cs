namespace CommonDomain
{
	using System;

	public interface IMemento<T> where T : struct
	{
		T Id { get; set; }
		int Version { get; set; }
	}
}