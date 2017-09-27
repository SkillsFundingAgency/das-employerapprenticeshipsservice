using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Tasks.API.Client;
using SFA.DAS.Tasks.API.Types.DTOs;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskApiClient _apiClient;
        private readonly ILog _logger;

        public TaskService(ITaskApiClient apiClient, ILog logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskDto>> GetAccountTasks(string accountId)
        {
            try
            {
                return await _apiClient.GetTasks(accountId);
            }
            catch (Exception ex)
            {
               _logger.Error(ex, "Could not retrieve account tasks successfully");
            }

            //TODO: Remove this mock code once the UI changes have been completed
            // We are adding it as a temporary workaround to get the UI changes done in time

            return new []
            {
                new TaskDto
                {
                    OwnerId = accountId,
                    ItemsDueCount = 2,
                    Type = "LevyDeclarationDue"
                },
                new TaskDto
                {
                    OwnerId = accountId,
                    ItemsDueCount = 2,
                    Type = "AgreementToSign"
                },
                new TaskDto
                {
                    OwnerId = accountId,
                    ItemsDueCount = 2,
                    Type = "AddApprentices"
                },
                new TaskDto
                {
                    OwnerId = accountId,
                    ItemsDueCount = 2,
                    Type = "ApprenticeChangesToReview"
                },
                new TaskDto
                {
                    OwnerId = accountId,
                    ItemsDueCount = 2,
                    Type = "CohortRequestReadyForApproval"
                },
                new TaskDto
                {
                    OwnerId = accountId,
                    ItemsDueCount = 2,
                    Type = "IncompleteApprenticeshipDetails"
                }
            };

        }
    }
}
