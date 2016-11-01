using MediatR;

namespace SFA.DAS.EAS.Application.Commands.AddPayeWithExistingLegalEntity
{
    public class AddPayeToAccountForExistingLegalEntityCommand : IAsyncRequest
    {
        public long LegalEntityId { get; set; }
        public string EmpRef { get; set; }  
        public string ExternalUserId { get; set; }
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public string HashedId { get; set; }
    }
}