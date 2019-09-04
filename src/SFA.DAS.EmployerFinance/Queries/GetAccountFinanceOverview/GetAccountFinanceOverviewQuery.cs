using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview
{
    public class GetAccountFinanceOverviewQuery : IAuthorizationContextModel, IAsyncRequest<GetAccountFinanceOverviewResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
    }
}
