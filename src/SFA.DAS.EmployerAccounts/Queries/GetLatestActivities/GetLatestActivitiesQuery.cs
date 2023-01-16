﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;

public class GetLatestActivitiesQuery : IAuthorizationContextModel, IRequest<GetLatestActivitiesResponse>
{
    [IgnoreMap]
    [Required]
    public long AccountId { get; set; }
}