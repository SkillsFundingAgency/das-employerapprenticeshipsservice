using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementQueryHandler : IAsyncRequestHandler<GetEmployerAgreementRequest, GetEmployerAgreementResponse>
    {
        private readonly EmployerAccountDbContext _database;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetEmployerAgreementRequest> _validator;

        public GetEmployerAgreementQueryHandler(
            EmployerAccountDbContext database,
            IHashingService hashingService,
            IValidator<GetEmployerAgreementRequest> validator)
        {
            _database = database;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async Task<GetEmployerAgreementResponse> Handle(GetEmployerAgreementRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

            var agreements = _database.Agreements.ToList();

            var employerAgreement = agreements.SingleOrDefault(x => x.Id.Equals(agreementId));

            EmployerAgreement lastSignedAgreement = null;

            if (employerAgreement != null && employerAgreement.StatusId == EmployerAgreementStatus.Pending)
            {
                lastSignedAgreement = agreements
                    .Where(x => x.AccountId.Equals(accountId) &&
                                x.LegalEntityId.Equals(employerAgreement.LegalEntityId) &&
                                x.StatusId == EmployerAgreementStatus.Signed)
                    .OrderByDescending(x => x.Template.VersionNumber)
                    .FirstOrDefault();
            }

            return new GetEmployerAgreementResponse
            {
                EmployerAgreement = employerAgreement,
                LastSignedAgreement = lastSignedAgreement
            };

            //TODO: work out why this is needed before creating PR?
            //if (agreement.Status != EmployerAgreementStatus.Signed)
            //{
            //    var user = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);
            //    agreement.SignedByName = $"{user.FirstName} {user.LastName}";
            //}
        }
    }
}