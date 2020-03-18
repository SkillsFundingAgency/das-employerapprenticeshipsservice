using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementType;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement
{
    public class GetEmployerAgreementTypeQueryHandler : IAsyncRequestHandler<GetEmployerAgreementTypeRequest, GetEmployerAgreementTypeResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetEmployerAgreementTypeRequest> _validator;

        public GetEmployerAgreementTypeQueryHandler(
            Lazy<EmployerAccountsDbContext> db,
            IHashingService hashingService,
            IValidator<GetEmployerAgreementTypeRequest> validator
        )
        {
            _db = db;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async Task<GetEmployerAgreementTypeResponse> Handle(GetEmployerAgreementTypeRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

            var agreementType = await _db.Value.Agreements
                .Where(x => x.Id == agreementId)
                .Select(x => x.Template.AgreementType).SingleAsync();

            return new GetEmployerAgreementTypeResponse
            {
                AgreementType = agreementType
            };
        }
    }
}