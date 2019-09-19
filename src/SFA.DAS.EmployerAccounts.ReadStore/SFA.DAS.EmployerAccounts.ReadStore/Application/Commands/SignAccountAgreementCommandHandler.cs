using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class SignAccountAgreementCommandHandler : IReadStoreRequestHandler<SignAccountAgreementCommand, Unit>
    {
        private readonly IAccountSignedAgreementsRepository _accountSignedAgreementsRepository;

        public SignAccountAgreementCommandHandler(IAccountSignedAgreementsRepository accountSignedAgreementsRepository)
        {
            _accountSignedAgreementsRepository = accountSignedAgreementsRepository;
        }

        public async Task<Unit> Handle(SignAccountAgreementCommand request, CancellationToken cancellationToken)
        {
            var accountSignedAgreement = await _accountSignedAgreementsRepository.CreateQuery().SingleOrDefaultAsync(x => x.AccountId == request.AccountId && x.AgreementVersion == request.AgreementVersion && x.AgreementType == request.AgreementType, cancellationToken);

            if (accountSignedAgreement == null)
            {
                accountSignedAgreement = new AccountSignedAgreement(request.AccountId, request.AgreementVersion, request.AgreementType, Guid.NewGuid());
                await _accountSignedAgreementsRepository.Add(accountSignedAgreement, null, cancellationToken);
            }

            return Unit.Value;
        }
    }
}