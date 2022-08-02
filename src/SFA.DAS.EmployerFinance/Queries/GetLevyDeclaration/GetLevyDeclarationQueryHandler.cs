using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryHandler : IAsyncRequestHandler<GetLevyDeclarationRequest, GetLevyDeclarationResponse>
    {
        private readonly IDasLevyRepository _repository;
        private readonly IValidator<GetLevyDeclarationRequest> _validator;
        private readonly IHashingService _hashingService;

        public GetLevyDeclarationQueryHandler(IDasLevyRepository repository, IValidator<GetLevyDeclarationRequest> validator, IHashingService hashingService)        
        {
            _repository = repository;
            _validator = validator;
            _hashingService = hashingService;
        }

        public async Task<GetLevyDeclarationResponse> Handle(GetLevyDeclarationRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var declarations = await _repository.GetAccountLevyDeclarations(accountId);

            return new GetLevyDeclarationResponse { Declarations = declarations };
        }
    }
}
