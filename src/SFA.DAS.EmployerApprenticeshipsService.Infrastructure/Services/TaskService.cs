using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        public IEnumerable<TaskDto> GetAccountTasks(string accountId)
        {
            throw new NotImplementedException();
        }
    }
}
