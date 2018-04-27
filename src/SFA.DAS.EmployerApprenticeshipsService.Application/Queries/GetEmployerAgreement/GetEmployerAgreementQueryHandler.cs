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
            var agreementId = _hashingService.DecodeValue(message.AgreementId);

            var employerAgreement = await _database.Agreements.ProjectTo<EmployerAgreementDto>(_configurationProvider)
                                                              .SingleOrDefaultAsync(x => x.Id.Equals(agreementId));

            if (employerAgreement == null)
                return new GetEmployerAgreementResponse();

            EmployerAgreementDto lastSignedAgreement = null;

            if (employerAgreement.StatusId == EmployerAgreementStatus.Pending)
            {
                lastSignedAgreement = _database.Agreements
                                               .OrderByDescending(x => x.Template.VersionNumber)
                                               .ProjectTo<EmployerAgreementDto>(_configurationProvider)
                                               .FirstOrDefault(x => x.AccountId.Equals(accountId) &&
                                                                    x.LegalEntityId.Equals(employerAgreement.LegalEntityId) &&
                                                                    x.StatusId == EmployerAgreementStatus.Signed);
            }

            if (employerAgreement.StatusId != EmployerAgreementStatus.Signed)
            {
                var currentUser = (from user in _database.Users
                                   join member in _database.Memberships on user.Id equals member.UserId
                                   where user.ExternalId.ToString().Equals(message.ExternalUserId) &&
                                         member.AccountId.Equals(accountId)
                                   select user).Single();

                employerAgreement.SignedByName = currentUser.FullName;
            }


            return new GetEmployerAgreementResponse
            {
                EmployerAgreement = employerAgreement,
                LastSignedAgreement = lastSignedAgreement
            };
        }
    }
}