using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandHandler : AsyncRequestHandler<RefreshEmployerLevyDataCommand>
    {
        [QueueName]
        public string refresh_employer_levy { get; set; }

        private readonly IValidator<RefreshEmployerLevyDataCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly IMessagePublisher _messagePublisher;

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, 
            IDasLevyRepository dasLevyRepository, IEnglishFractionRepository englishFractionRepository, 
            IMessagePublisher messagePublisher)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _englishFractionRepository = englishFractionRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(RefreshEmployerLevyDataCommand message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            bool sendLevyDataChanged = false;
            foreach (var employerLevyData in message.EmployerLevyData)
            {
                foreach (var dasDeclaration in employerLevyData.Declarations.Declarations.OrderBy(c=>c.Date))
                {
                    var declaration = await _dasLevyRepository.GetEmployerDeclaration(dasDeclaration.Id, employerLevyData.EmpRef);

                    if (declaration == null)
                    {
                        if (dasDeclaration.NoPaymentForPeriod)
                        {
                            var previousSubmission = await _dasLevyRepository.GetLastSubmissionForScheme(employerLevyData.EmpRef);
                            dasDeclaration.LevyDueYtd = previousSubmission.LevyDueYtd;
                            dasDeclaration.LevyAllowanceForFullYear = previousSubmission.LevyAllowanceForFullYear;
                        }

                        await _dasLevyRepository.CreateEmployerDeclaration(dasDeclaration, employerLevyData.EmpRef, message.AccountId);
                        sendLevyDataChanged = true;
                    }
                }

                foreach (var fraction in employerLevyData.Fractions.Fractions)
                {
                    var dasFraction = await _englishFractionRepository.GetEmployerFraction(fraction.DateCalculated, employerLevyData.EmpRef);

                    if (dasFraction == null)
                    {
                        await _englishFractionRepository.CreateEmployerFraction(fraction, employerLevyData.EmpRef);
                        sendLevyDataChanged = true;
                    }
                }
            }

            if (sendLevyDataChanged)
            {
                await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage { AccountId = message.AccountId });
            }

        }
    }
}
