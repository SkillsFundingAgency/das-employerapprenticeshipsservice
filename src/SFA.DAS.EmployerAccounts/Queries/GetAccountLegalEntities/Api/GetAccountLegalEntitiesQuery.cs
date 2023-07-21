using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

public class GetAccountLegalEntitiesQuery : IRequest<GetAccountLegalEntitiesResponse>
{
    [Required]
    [Range(1, int.MaxValue)]
    public int? PageNumber { get; set; } = 1;

    [Required]
    [Range(100, 10000)]
    public int? PageSize { get; set; } = 1000;
}