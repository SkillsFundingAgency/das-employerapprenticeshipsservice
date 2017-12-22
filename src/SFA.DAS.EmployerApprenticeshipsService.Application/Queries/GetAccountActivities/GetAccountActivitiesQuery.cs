using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.Activities;

namespace SFA.DAS.EAS.Application.Queries.GetAccountActivities
{
    public class GetAccountActivitiesQuery : IAsyncRequest<GetAccountActivitiesResponse>
    {
        public long AccountId { get; set; }
        public int? Take { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Term { get; set; }
        public ActivityTypeCategory? Category { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}