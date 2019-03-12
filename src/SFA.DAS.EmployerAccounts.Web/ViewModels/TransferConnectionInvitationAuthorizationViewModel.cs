using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferConnectionInvitationAuthorizationViewModel
    {
        public AuthorizationResult AuthorizationResult { get; set; }
        public bool IsValidSender { get; set; }
        public decimal PercentLevyTransferAllowance { get => _PercentLevyTransferAllowance * 100; set => _PercentLevyTransferAllowance = value; }
        private decimal _PercentLevyTransferAllowance;
    }
}