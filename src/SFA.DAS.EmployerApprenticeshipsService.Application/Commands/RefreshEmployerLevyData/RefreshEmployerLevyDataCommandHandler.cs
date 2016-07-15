using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandHandler : AsyncRequestHandler<RefreshEmployerLevyDataCommand>
    {
        private readonly IValidator<RefreshEmployerLevyDataCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMessagePublisher _messagePublisher; 

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository, IMessagePublisher messagePublisher)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
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
                foreach (var dasDeclaration in employerLevyData.Declarations.Declarations)
                {
                    var declaration = await _dasLevyRepository.GetEmployerDeclaration(dasDeclaration.Id, employerLevyData.EmpRef);

                    if (declaration == null)
                    {
                        await _dasLevyRepository.CreateEmployerDeclaration(dasDeclaration, employerLevyData.EmpRef);
                        sendLevyDataChanged = true;
                    }
                }

                var fraction = await _dasLevyRepository.GetEmployerFraction(employerLevyData.Fractions.DateCalculated, employerLevyData.EmpRef);

                if (fraction == null)
                {
                    await _dasLevyRepository.CreateEmployerFraction(employerLevyData.Fractions, employerLevyData.EmpRef);
                    sendLevyDataChanged = true;
                }
            }

            if (sendLevyDataChanged)
            {
                await _messagePublisher.PublishAsync(new EmployerRefreshLevyViewsQueueMessage {Id = message.EmployerId});
            }




            /*
             TODO
             Check to see if it exists in the Levy Dec store
             Check to see if it exists in the Fraction store

             if we need to insert
                Then transform dates - from Tax year to calendar year
                Insert new rows for dec and fraction
                
             Trigger an event for refresh of data for the BuildTransaction view -> put a message in a queue

             */

        }
    }
}
