using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure
{
	public class TriggeredJobRepository : ITriggeredJobRepository
	{
		public IEnumerable<TriggeredJob<QueueTriggerAttribute>> GetQueuedTriggeredJobs()
		{
			return GetTriggeredJob<QueueTriggerAttribute>();
		}

		public IEnumerable<TriggeredJob<TimerTriggerAttribute>> GetScheduledJobs()
		{
			return GetTriggeredJob<TimerTriggerAttribute>();
		}

		private IEnumerable<TriggeredJob<TTriggerAttribute>> GetTriggeredJob<TTriggerAttribute>() where TTriggerAttribute : Attribute
		{ 
			return Assembly.GetExecutingAssembly().GetTypes().SelectMany(t =>
				t.GetMethods().SelectMany(method => method.GetParameters().Where(p => ((ParameterInfo) p).IsDefined(typeof(TTriggerAttribute))))).Select(
				p => new TriggeredJob<TTriggerAttribute>
				{
					TriggerParameter = p,
					Trigger = (TTriggerAttribute) p.GetCustomAttribute(typeof(TTriggerAttribute)),
					InvokedMethod = p.Member as MethodInfo,
					ContainingClass = p.Member.DeclaringType
				});
		}
	}
}