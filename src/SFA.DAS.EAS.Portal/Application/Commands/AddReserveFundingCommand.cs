using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Portal.Application.Commands
{
    public class AddReserveFundingCommand
    {
        private readonly ILogger _logger;

        public AddReserveFundingCommand(ILogger logger)
        {
            _logger = logger;
        }

        //todo: accept event directly?
        public Task Execute(long accountId, long accountLegalEntityId, string legalEntityName, long courseId, string courseName, DateTime startDate, DateTime endDate, DateTime eventCreated)
        {
            _logger.LogInformation("Executing AddReserveFundingCommand");
            return Task.CompletedTask;
        }
    }
}
