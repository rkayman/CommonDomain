namespace CommonDomain.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public class ConventionMessageRouter<T> : IRouteMessages<T> where T : struct
	{
		private readonly bool throwOnApplyNotFound;
		private readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
		private IAggregate<T> registered;

		public ConventionMessageRouter() : this(true) { }

		public ConventionMessageRouter(bool throwOnApplyNotFound)
		{
			this.throwOnApplyNotFound = throwOnApplyNotFound;
		}

		public ConventionMessageRouter(bool throwOnApplyNotFound, IAggregate<T> aggregate)
			: this(throwOnApplyNotFound)
		{
			Register(aggregate);
		}

		public void Register<TMessage>(Action<TMessage> handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");

			handlers[typeof(TMessage)] = message => handler((TMessage) message);
		}

		public void Register(IAggregate<T> aggregate)
		{
			if (aggregate == null)
				throw new ArgumentNullException("aggregate");

			registered = aggregate;

			var applyMethods = GetInstanceMethodsMatchingMessageSignature(aggregate);

			foreach (var apply in applyMethods)
			{
				var applyMethod = apply.Method;
				handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] { m }));
			}
		}

		public void Dispatch(object message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			Action<object> handler;

			if (handlers.TryGetValue(message.GetType(), out handler))
			{
				handler(message);
			}
			else if (throwOnApplyNotFound)
			{
				registered.ThrowHandlerNotFound(message);
			}
		}

		protected class MatchingMessageMethod
		{
			public MethodInfo Method { get; set; }

			public Type MessageType { get; set; }

			public MatchingMessageMethod(MethodInfo method, Type parameterType)
			{
				Method = method;
				MessageType = parameterType;
			}
		}

		private static IEnumerable<MatchingMessageMethod> GetInstanceMethodsMatchingMessageSignature(IAggregate<T> aggregate)
		{
			// Get instance methods named Apply with one parameter returning void
			return aggregate.GetType()
			                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			                .Where(m => m != null &&
			                            (m.Name == "Apply" && m.GetParameters().Length == 1 &&
			                             (m.ReturnParameter != null &&
			                              m.ReturnParameter.ParameterType == typeof(void))))
			                .Select(m => new MatchingMessageMethod(m, m.GetParameters().Single().ParameterType));
		}
	}
}