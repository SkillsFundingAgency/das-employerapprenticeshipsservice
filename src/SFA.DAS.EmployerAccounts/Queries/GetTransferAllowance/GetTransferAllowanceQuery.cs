using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceQuery : IAsyncRequest<GetTransferAllowanceResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
    }
}