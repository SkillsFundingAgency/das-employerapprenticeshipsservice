using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTransferRole
{
    public class GetAccountTransferRoleResponse
    {
        public bool IsPendingReceiver { get; set; }
        public bool IsApprovedReceiver { get; set; }
    }

}