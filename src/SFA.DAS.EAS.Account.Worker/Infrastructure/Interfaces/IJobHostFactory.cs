﻿using Microsoft.Azure.WebJobs;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces
{
	/// <summary>
	///		Factory for creating jobhosts
	/// </summary>
	public interface IJobHostFactory
	{
		JobHost CreateJobHost();
	}
}