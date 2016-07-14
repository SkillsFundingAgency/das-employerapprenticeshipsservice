using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandHandler : AsyncRequestHandler<RefreshEmployerLevyDataCommand>
    {
        private readonly IValidator<RefreshEmployerLevyDataCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        protected override async Task HandleCore(RefreshEmployerLevyDataCommand message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }


            foreach (var dasDeclaration in message.Declarations.Declarations)
            {
                var declaration = await _dasLevyRepository.GetEmployerDeclaration(dasDeclaration.Id, message.EmpRef);

                if (declaration == null)
                {
                    await _dasLevyRepository.CreateEmployerDeclaration(dasDeclaration);
                }
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
