using System;
using System.Reflection;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
	public class TriggeredJob<TAttribute>  where TAttribute : Attribute
	{
		public Type ContainingClass { get; set; }
		public MethodInfo InvokedMethod { get; set; }
		public TAttribute Trigger { get; set; }
		public ParameterInfo TriggerParameter { get; set; }
	}
}