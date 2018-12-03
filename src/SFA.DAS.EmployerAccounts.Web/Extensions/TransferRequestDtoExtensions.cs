using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
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