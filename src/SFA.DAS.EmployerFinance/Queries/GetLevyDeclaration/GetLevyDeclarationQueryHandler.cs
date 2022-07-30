using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryHandler : IAsyncRequestHandler<GetLevyDeclarationRequest, GetLevyDeclarationResponse>
    {
        private readonly IDasLevyRepository _repository;
        private readonly IValidator<GetLevyDeclarationRequest> _validator;
        //private readonly IHashingService _hashingService;

        //public GetLevyDeclarationQueryHandler(IDasLevyRepository repository, IValidator<GetLevyDeclarationRequest> validator, IHashingService hashingService)
        public GetLevyDeclarationQueryHandler(IDasLevyRepository repository, IValidator<GetLevyDeclarationRequest> validator)
        {
            _repository = repository;
            _validator = validator;
            //_hashingService = hashingService;
        }

        public async Task<GetLevyDeclarationResponse> Handle(GetLevyDeclarationRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            //TODO : inlcude this later
            //var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            //accountId = 103960
            //var declarations = await _repository.GetAccountLevyDeclarations(accountId);
            var declarations = await _repository.GetAccountLevyDeclarations(103960);

            return new GetLevyDeclarationResponse { Declarations = declarations };
        }
    }
}
