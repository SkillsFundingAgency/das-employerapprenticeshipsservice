namespace SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces
{
	/// <summary>
	///		A service that can support webjobs by performing certain setup functions.
	/// </summary>
	public interface IAzureWebJobHelper
	{
		/// <summary>
		///		Ensures that all queues that are referenced in queue triggers exist. 
		/// </summary>
		void EnsureAllQueuesForTriggeredJobs();
	}
}