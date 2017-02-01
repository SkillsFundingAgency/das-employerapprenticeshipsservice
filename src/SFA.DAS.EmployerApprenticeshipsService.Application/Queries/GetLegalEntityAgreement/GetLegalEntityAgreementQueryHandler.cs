using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement
{
    public class GetLegalEntityAgreementQueryHandler : IAsyncRequestHandler<GetLegalEntityAgreementRequest, GetLegalEntityAgreementResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public GetLegalEntityAgreementQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<GetLegalEntityAgreementResponse> Handle(GetLegalEntityAgreementRequest message)
        {
            var agreements = await _accountRepository.GetEmployerAgreementsLinkedToAccount(message.AccountId);

            var legalEntityAgreement = agreements.SingleOrDefault(x => x.LegalEntityCode != null &&
                                                                       x.LegalEntityCode.Equals(message.LegalEntityCode) &&
                                                                       x.Status == EmployerAgreementStatus.Pending);

            return new GetLegalEntityAgreementResponse()
            {
                EmployerAgreement = legalEntityAgreement
            };
        }
    }
}
