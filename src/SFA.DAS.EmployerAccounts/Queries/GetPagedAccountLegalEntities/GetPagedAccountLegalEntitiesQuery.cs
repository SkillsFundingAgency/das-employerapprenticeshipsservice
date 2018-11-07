using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetPagedAccountLegalEntities
{
    public class GetPagedAccountLegalEntitiesQuery : IAsyncRequest<GetPagedAccountLegalEntitiesResponse>
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int? PageNumber { get; set; } = 1;

        [Required]
        [Range(100, 10000)]
        public int? PageSize { get; set; } = 1000;
    }
}