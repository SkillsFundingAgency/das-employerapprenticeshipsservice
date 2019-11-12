using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement
{
    public class GetUnsignedEmployerAgreementQueryHandler : IAsyncRequestHandler<GetUnsignedEmployerAgreementRequest, GetUnsignedEmployerAgreementResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetUnsignedEmployerAgreementRequest> _validator;

        public GetUnsignedEmployerAgreementQueryHandler(
            Lazy<EmployerAccountsDbContext> db,
            IHashingService hashingService,
            IValidator<GetUnsignedEmployerAgreementRequest> validator
        )
        {
            _db = db;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async Task<GetUnsignedEmployerAgreementResponse> Handle(GetUnsignedEmployerAgreementRequest message)
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

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            var accountLegalEntities = _db.Value.AccountLegalEntities.ToList();

            var pendingAgreementId = await _db.Value.AccountLegalEntities
                .Where(x => x.AccountId == accountId && x.PendingAgreementId != null)
                .Select(x => x.PendingAgreementId).SingleOrDefaultAsync();

            return new GetUnsignedEmployerAgreementResponse
            {
                HashedAgreementId = pendingAgreementId.HasValue ? _hashingService.HashValue(pendingAgreementId.Value) : null
            };
        }
    }
}