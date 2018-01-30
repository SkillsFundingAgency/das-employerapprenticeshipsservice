using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Activities;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQuery : IAsyncRequest<GetActivitiesResponse>
    {
        [Required]
        [RegularExpression(@"^[A-Za-z\d]{6,6}$")]
        public string HashedAccountId { get; set; }

        public int? Take { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Term { get; set; }
        public ActivityTypeCategory? Category { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}