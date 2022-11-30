using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class TransferRequestsViewModel : IAuthorizationContextModel
    {
        public long AccountId { get; set; }

        public IEnumerable<TransferRequestDto> TransferRequests { get; set; }

        public IEnumerable<TransferRequestDto> TransferSenderRequests =>
            TransferRequests.Where(p => p.SenderAccount.Id == AccountId);

        public IEnumerable<TransferRequestDto> TransferReceiverRequests =>
            TransferRequests.Where(p => p.ReceiverAccount.Id == AccountId);
    }
}