using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTransferTransactionDetails
{
    public class GetSenderTransferTransactionDetailsQuery : MembershipMessage, IAsyncRequest<GetSenderTransferTransactionDetailsResponse>
    {
        [IgnoreMap]
        [Required]
        public long ReceiverAccountId { get; set; }

        [Required]
        public string PeriodEnd { get; set; }
    }
}
