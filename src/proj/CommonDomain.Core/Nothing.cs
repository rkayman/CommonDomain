namespace CommonDomain.Core
{
	public class Nothing
	{
		public static readonly Nothing Value = new Nothing();

		private Nothing() { }

		public override string ToString()
		{
			return "Nothing";
		}
	}
}