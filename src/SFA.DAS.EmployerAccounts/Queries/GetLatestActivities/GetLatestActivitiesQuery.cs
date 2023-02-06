using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration.Annotations;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;

public class GetLatestActivitiesQuery : IAuthorizationContextModel, IRequest<GetLatestActivitiesResponse>
{
    [Ignore]
    [Required]
    public long AccountId { get; set; }
}