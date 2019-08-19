using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.LegalEntitySignAgreement
{
    public class LegalEntitySignAgreementCommand : IAsyncRequest
    {
        public LegalEntitySignAgreementCommand(long signedAgreementId, int signedAgreementVersion, long accountId,
            long legalEntityId)
        {
            SignedAgreementId = signedAgreementId;
            SignedAgreementVersion = signedAgreementVersion;
            AccountId = accountId;
            LegalEntityId = legalEntityId;
        }


        public long SignedAgreementId { get; set; }
        public int SignedAgreementVersion { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
    }

    public class LegalEntitySignAgreementCommandHandler : AsyncRequestHandler<LegalEntitySignAgreementCommand>
    {
        private readonly IAccountLegalEntityRepository _accountLegalEntityRepository;
        private readonly ILog _logger;

        public LegalEntitySignAgreementCommandHandler(IAccountLegalEntityRepository accountLegalEntityRepository, ILog logger)
        {
            _accountLegalEntityRepository = accountLegalEntityRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(LegalEntitySignAgreementCommand message)
        {
            try
            {
                await _accountLegalEntityRepository.SignAgreement(message.SignedAgreementId, message.SignedAgreementVersion,
                    message.AccountId, message.LegalEntityId);
                _logger.Info($"Signed agreement on legal entity {message.LegalEntityId}");
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Could not sign agreement on legal entity");
                throw;
            }
        }
    }
}