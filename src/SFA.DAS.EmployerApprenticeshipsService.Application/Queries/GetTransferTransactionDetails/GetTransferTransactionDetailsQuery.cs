using MediatR;
using SFA.DAS.EAS.Application.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EAS.Application.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsQuery : MembershipMessage, IAsyncRequest<GetTransferTransactionDetailsResponse>
    {

        [Required]
        public string TargetAccountPublicHashedId { get; set; }

        [Required]
        public string PeriodEnd { get; set; }
    }
}