using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Web.Extensions
{
    public static class TransferRequestDtoExtensions
    {
        public static AccountDto GetPeerAccount(this TransferRequestDto transferRequest, long accountId)
        {
            return transferRequest.SenderAccount.Id == accountId
                ? transferRequest.ReceiverAccount
                : transferRequest.SenderAccount;
        }
    }
}