using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
	public class TriggeredJobRepository : ITriggeredJobRepository
	{
        private readonly Lazy<IEnumerable<TriggeredJob<QueueTriggerAttribute>>> _queueTriggeredJobs = new Lazy<IEnumerable<TriggeredJob<QueueTriggerAttribute>>>(GetTriggeredJob<QueueTriggerAttribute>);

	    private readonly Lazy<IEnumerable<TriggeredJob<TimerTriggerAttribute>>> _timerTriggeredJobs = new Lazy<IEnumerable<TriggeredJob<TimerTriggerAttribute>>>(GetTriggeredJob<TimerTriggerAttribute>);

        public IEnumerable<TriggeredJob<QueueTriggerAttribute>> GetQueuedTriggeredJobs()
        {
            return _queueTriggeredJobs.Value;
        }

		public IEnumerable<TriggeredJob<TimerTriggerAttribute>> GetScheduledJobs()
		{
		    return _timerTriggeredJobs.Value;
		}

        private static IEnumerable<TriggeredJob<TTriggerAttribute>> GetTriggeredJob<TTriggerAttribute>() where TTriggerAttribute : Attribute
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