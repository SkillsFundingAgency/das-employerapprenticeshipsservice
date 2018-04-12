using System.Collections.Generic;
using Microsoft.Azure.WebJobs;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces
{
	/// <summary>
	///		A repository for discovering all jobs triggered by Azure SDK triggers.
	/// </summary>
	public interface ITriggeredJobRepository
	{
		/// <summary>
		///		Returns all tasks that are triggered by a queue message (on any queue)
		/// </summary>
		/// <returns></returns>
		IEnumerable<TriggeredJob<QueueTriggerAttribute>> GetQueueuTriggeredJobs();

		/// <summary>
		///		Returns all tasks that are triggered on a scheduled timer
		/// </summary>
		/// <returns></returns>
		IEnumerable<TriggeredJob<TimerTriggerAttribute>> GetScheduledJobs();
	}
}