using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationRequestHandler : IAsyncRequestHandler<GetLastLevyDeclarationRequest, GetLastLevyDeclarationResponse>
    {
        private readonly IValidator<GetLastLevyDeclarationRequest> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        
        public GetLastLevyDeclarationRequestHandler(IValidator<GetLastLevyDeclarationRequest> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetLastLevyDeclarationResponse> Handle(GetLastLevyDeclarationRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var result = await _dasLevyRepository.GetLastSubmissionForScheme(message.Empref);
            
            return new GetLastLevyDeclarationResponse {Transaction = result };
        }
    }
}