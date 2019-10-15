using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Activities;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQuery : IAuthorizationContextModel, IAsyncRequest<GetActivitiesResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
        public int? Take { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Term { get; set; }
        public ActivityTypeCategory? Category { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}