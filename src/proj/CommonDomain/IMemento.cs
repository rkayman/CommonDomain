namespace CommonDomain
{
	using System;

	public interface IMemento
	{
		long Id { get; set; }
		Guid Guid { get; set; }
		int Version { get; set; }
	}
}