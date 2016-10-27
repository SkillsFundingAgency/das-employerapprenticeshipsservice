using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.LevyAccountUpdater.Updater
{
    public class AccountUpdater : IAccountUpdater
    {
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;

        [QueueName]
        public string get_employer_levy { get; set; }

        public AccountUpdater(IEmployerAccountRepository accountRepository, IMessagePublisher messagePublisher)
        {
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task RunUpdate()
        {
            var employerAccounts = await _accountRepository.GetAllAccounts();

            var tasks = employerAccounts.Select(
                x => _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage {AccountId = x.Id})).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}
