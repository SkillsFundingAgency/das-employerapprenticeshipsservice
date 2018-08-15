using MediatR;
using SFA.DAS.EmployerFinance.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsQuery : AccountMessage, IAsyncRequest<GetTransferTransactionDetailsResponse>
    {

        [Required]
        public string TargetAccountPublicHashedId { get; set; }

        [Required]
        public string PeriodEnd { get; set; }
    }
}