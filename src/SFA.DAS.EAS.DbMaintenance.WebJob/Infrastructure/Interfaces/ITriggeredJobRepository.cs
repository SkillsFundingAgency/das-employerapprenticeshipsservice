using System.Collections.Generic;
using Microsoft.Azure.WebJobs;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces
{
	/// <summary>
	///		A repository for discovering all jobs triggered by Azure SDK triggers.
	/// </summary>
	/// <remarks>
	///		Currently only looks for Queue Triggers because that's all we have in our code base.
	/// </remarks>
	public interface ITriggeredJobRepository
	{
		IEnumerable<TriggeredJob<QueueTriggerAttribute>> GetQueueuTriggeredJobs();
	}
}