using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementQueryHandler : IAsyncRequestHandler<GetEmployerAgreementRequest, GetEmployerAgreementResponse>
    {
        private readonly EmployerAccountDbContext _database;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetEmployerAgreementRequest> _validator;
        private readonly IConfigurationProvider _configurationProvider;

        public GetEmployerAgreementQueryHandler(
            EmployerAccountDbContext database,
            IHashingService hashingService,
            IValidator<GetEmployerAgreementRequest> validator,
            IConfigurationProvider configurationProvider)
        {
            _database = database;
            _hashingService = hashingService;
            _validator = validator;
            _configurationProvider = configurationProvider;
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

            var employerAgreement = await _database.Agreements.ProjectTo<AgreementDto>(_configurationProvider)
                                                              .SingleOrDefaultAsync(x => x.Id.Equals(agreementId));

            if (employerAgreement == null)
                return new GetEmployerAgreementResponse();

            AgreementDto lastSignedAgreement = null;

            if (employerAgreement.StatusId == EmployerAgreementStatus.Pending)
            {
                lastSignedAgreement = _database.Agreements
                                               .OrderByDescending(x => x.Template.VersionNumber)
                                               .ProjectTo<AgreementDto>(_configurationProvider)
                                               .FirstOrDefault(x => x.AccountId.Equals(accountId) &&
                                                                    x.LegalEntityId.Equals(employerAgreement.LegalEntityId) &&
                                                                    x.StatusId == EmployerAgreementStatus.Signed);
            }

            if (employerAgreement.StatusId != EmployerAgreementStatus.Signed)
            {
                employerAgreement.SignedByName = _database.Memberships
                    .Where(m => m.AccountId == accountId && m.User.ExternalId.ToString() == message.ExternalUserId)
                    .Select(m => m.User.FullName)
                    .Single();
            }

            employerAgreement.HashedAccountId = _hashingService.HashValue(employerAgreement.AccountId);
            employerAgreement.HashedAgreementId = _hashingService.HashValue(employerAgreement.Id);

            if (lastSignedAgreement != null)
            {
                lastSignedAgreement.HashedAccountId = _hashingService.HashValue(lastSignedAgreement.AccountId);
                lastSignedAgreement.HashedAgreementId = _hashingService.HashValue(lastSignedAgreement.Id);
            }

            return new GetEmployerAgreementResponse
            {
                EmployerAgreement = employerAgreement,
                LastSignedAgreement = lastSignedAgreement
            };
        }
    }
}