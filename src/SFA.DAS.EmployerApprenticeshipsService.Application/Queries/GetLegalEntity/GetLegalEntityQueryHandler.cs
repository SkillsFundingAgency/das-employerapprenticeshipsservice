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
            var legalEntity = await _db.LegalEntities
                .Where(l =>
                    l.Id == message.LegalEntityId.Value &&
                    l.Agreements.Any(a =>
                        a.Account.Id == message.AccountId.Value && (
                        a.StatusId == EmployerAgreementStatus.Pending ||
                        a.StatusId == EmployerAgreementStatus.Signed)))
                .ProjectTo<LegalEntityViewModel>(_configurationProvider, new
                {
                    accountId = message.AccountId.Value,
                    accountHashedId = message.AccountHashedId
                })
                .SingleOrDefaultAsync();

            return new GetLegalEntityResponse
            {
                LegalEntity = legalEntity
            };
        }
    }
}