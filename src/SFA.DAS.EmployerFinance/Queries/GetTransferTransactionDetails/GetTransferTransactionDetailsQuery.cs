using MediatR;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;

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