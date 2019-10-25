using MediatR;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsQuery : IAuthorizationContextModel, IAsyncRequest<GetTransferTransactionDetailsResponse>
    {
        [IgnoreMap]
        [Required]
        public long? AccountId { get; set; }

        [Required]
        public string TargetAccountPublicHashedId { get; set; }

        [Required]
        public string PeriodEnd { get; set; }
    }
}