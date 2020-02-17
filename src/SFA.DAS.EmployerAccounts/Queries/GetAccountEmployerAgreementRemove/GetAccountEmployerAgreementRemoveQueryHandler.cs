using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove
{
    public class GetAccountEmployerAgreementRemoveQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementRemoveRequest, GetAccountEmployerAgreementRemoveResponse>
    {
        private readonly IValidator<GetAccountEmployerAgreementRemoveRequest> _validator;
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IHashingService _hashingService;

        public GetAccountEmployerAgreementRemoveQueryHandler(IValidator<GetAccountEmployerAgreementRemoveRequest> validator, IHashingService hashingService, Lazy<EmployerAccountsDbContext> db)
        {
            _validator = validator;
            _hashingService = hashingService;
            _db = db;
        }

        public async Task<GetAccountEmployerAgreementRemoveResponse> Handle(GetAccountEmployerAgreementRemoveRequest message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }
            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

            var agreement = await _db.Value.Agreements.Where(x => x.Id == agreementId).Select(x => new {x.Id, x.StatusId, x.AccountLegalEntity.Name}).SingleOrDefaultAsync();
            
            if (agreement == null)
            {
                return new GetAccountEmployerAgreementRemoveResponse();
            }

            var agreementView = new RemoveEmployerAgreementView
            {
                CanBeRemoved = true,
                HashedAccountId = message.HashedAccountId,
                HashedAgreementId = message.HashedAgreementId,
                Id = agreement.Id,
                Name = agreement.Name,
                Status = agreement.StatusId
            };

            return new GetAccountEmployerAgreementRemoveResponse {Agreement = agreementView};
        }
    }
}
