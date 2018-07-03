using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Infrastructure.Data;
using EmployerAgreementStatus = SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntity
{
    public class GetLegalEntityQueryHandler : IAsyncRequestHandler<GetLegalEntityQuery, GetLegalEntityResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetLegalEntityQueryHandler(EmployerAccountDbContext db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetLegalEntityResponse> Handle(GetLegalEntityQuery message)
        {
            var legalEntity = await _db.AccountLegalEntity
                .Where(l =>
                    l.LegalEntityId == message.LegalEntityId.Value &&
                    l.AccountId == message.AccountId.Value && 
                    (l.PendingAgreementId != null || l.SignedAgreementId != null))
                .ProjectTo<LegalEntityViewModel>(_configurationProvider, new
                {
                    accountId = message.AccountId.Value,
                    accountHashedId = message.AccountHashedId
                })
                .SingleOrDefaultAsync();

            var latestAgreement = legalEntity?.Agreements
                .OrderByDescending(a => a.TemplateVersionNumber)
                .FirstOrDefault();

            if (legalEntity != null && latestAgreement != null)
            {
                legalEntity.AgreementSignedByName = latestAgreement.SignedByName;
                legalEntity.AgreementSignedDate = latestAgreement.SignedDate;
                legalEntity.AgreementStatus = latestAgreement.Status;
            }

            return new GetLegalEntityResponse
            {
                LegalEntity = legalEntity
            };
        }
    }
}