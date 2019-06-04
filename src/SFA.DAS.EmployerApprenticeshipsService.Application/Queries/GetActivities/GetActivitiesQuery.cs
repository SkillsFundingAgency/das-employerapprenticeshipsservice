using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.Activities;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQuery : MembershipMessage, IAsyncRequest<GetActivitiesResponse>
    {
        public int? Take { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Term { get; set; }
        public ActivityTypeCategory? Category { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}