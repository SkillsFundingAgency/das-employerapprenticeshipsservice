using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQuery : IRequest<GetLatestActivitiesResponse>
    {
        [Required]
        [RegularExpression(Constants.HashedAccountIdRegex)]
        public string HashedAccountId { get; set; }
    }
}