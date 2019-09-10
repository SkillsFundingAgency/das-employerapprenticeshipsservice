using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class HasAgreementBeenSignedQueryHandler : IReadStoreRequestHandler<HasAgreementBeenSignedQuery, bool>
    {
        private readonly IAccountSignedAgreementsRepository _accountSignedAgreementsRepository;

        public HasAgreementBeenSignedQueryHandler(IAccountSignedAgreementsRepository accountSignedAgreementsRepository)
        {
            _accountSignedAgreementsRepository = accountSignedAgreementsRepository;
        }

        public Task<bool> Handle(HasAgreementBeenSignedQuery request, CancellationToken cancellationToken)
        {
            return _accountSignedAgreementsRepository
                .CreateQuery()
                .AnyAsync(r =>
                    r.AccountId == request.AccountId &&
                    r.AgreementType == request.AgreementType &&
                    r.AgreementVersion == request.AgreementVersion, cancellationToken);
        }
    }
}