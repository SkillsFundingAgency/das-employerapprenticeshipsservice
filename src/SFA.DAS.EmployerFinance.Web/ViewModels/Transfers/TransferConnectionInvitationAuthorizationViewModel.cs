using SFA.DAS.Authorization.Results;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class TransferConnectionInvitationAuthorizationViewModel
    {
        public AuthorizationResult AuthorizationResult { get; set; }
        public bool IsValidSender { get; set; }
        public decimal TransferAllowancePercentage { get => _TransferAllowancePercentage * 100; set => _TransferAllowancePercentage = value; }
        private decimal _TransferAllowancePercentage;
    }
}