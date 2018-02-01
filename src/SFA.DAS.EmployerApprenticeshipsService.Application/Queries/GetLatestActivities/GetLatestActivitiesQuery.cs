using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQuery : IRequest<GetLatestActivitiesResponse>
    {
        [Required]
        [RegularExpression(@"^[A-Za-z\d]{6,6}$")]
        public string HashedAccountId { get; set; }
    }
}