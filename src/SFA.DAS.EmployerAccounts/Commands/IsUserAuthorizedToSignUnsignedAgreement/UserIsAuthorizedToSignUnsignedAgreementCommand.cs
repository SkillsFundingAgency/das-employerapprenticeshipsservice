using MediatR;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Commands.IsUserAuthorizedToSignUnsignedAgreement
{
    public class UserIsAuthorizedToSignUnsignedAgreementCommand : IAsyncRequest<UserIsAuthorizedToSignUnsignedAgreementCommandResponse>
    {
        public EmployerAgreementView EmployerAgreement { get; set; }
        public GetEmployerAgreementRequest EmployerAgreementRequest { get; set; }
    }
}
