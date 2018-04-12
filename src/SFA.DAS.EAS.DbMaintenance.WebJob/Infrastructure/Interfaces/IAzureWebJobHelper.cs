namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces
{
	/// <summary>
	///		A service that can support webjobs by performing certain setup functions.
	/// </summary>
	public interface IAzureWebJobHelper
	{
		void EnsureAllQueuesForTriggeredJobs();
	}
}