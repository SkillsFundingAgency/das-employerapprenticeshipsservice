using MediatR;

namespace SFA.DAS.EAS.Application.Commands.RemoveLegalEntity
{
    public class RemoveLegalEntityCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
        public string HashedLegalAgreementId { get; set; }
    }
}
