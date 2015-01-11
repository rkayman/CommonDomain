namespace CommonDomain.Core
{
	using System.Globalization;

	internal static class ExtensionMethods
	{
		public static string FormatWith( this string format, params object[] args )
		{
			return string.Format( CultureInfo.InvariantCulture, format ?? string.Empty, args );
		}

		public static void ThrowHandlerNotFound<T>( this IAggregate<T> aggregate, object eventMessage ) where T : struct
		{
			var exceptionMessage = "Aggregate of type '{0}' raised an event of type '{1}' but not handler could be found to handle the message."
				.FormatWith( aggregate.GetType().Name, eventMessage.GetType().Name );

			throw new HandlerForDomainEventNotFoundException( exceptionMessage );
		}
	}
}