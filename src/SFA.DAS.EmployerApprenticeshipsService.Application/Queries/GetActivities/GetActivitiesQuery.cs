using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Activities;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQuery : IAsyncRequest<GetActivitiesResponse>
    {
        [Required]
        [RegularExpression(Constants.HashedAccountIdRegex)]
        public string HashedAccountId { get; set; }

        public int? Take { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Term { get; set; }
        public ActivityTypeCategory? Category { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}