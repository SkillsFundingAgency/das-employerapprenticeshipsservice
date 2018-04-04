using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EAS.Application.Queries.GetTransferSenderTransactionDetails
{
    public class GetTransferTransactionDetailsQuery : MembershipMessage, IAsyncRequest<GetTransferSenderTransactionDetailsResponse>
    {
        [IgnoreMap]
        [Required]
        public long TargetAccountId { get; set; }

        [Required]
        public string PeriodEnd { get; set; }
    }
}