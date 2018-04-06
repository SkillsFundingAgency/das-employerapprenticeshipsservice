using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsQuery : MembershipMessage, IAsyncRequest<GetTransferTransactionDetailsResponse>
    {
        [IgnoreMap]
        [Required]
        public long TargetAccountId { get; set; }

        [Required]
        public string PeriodEnd { get; set; }
    }
}