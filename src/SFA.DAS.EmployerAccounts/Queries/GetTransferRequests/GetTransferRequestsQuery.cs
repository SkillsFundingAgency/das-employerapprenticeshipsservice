﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferRequests
{
    public class GetTransferRequestsQuery : IAuthorizationContextModel, IAsyncRequest<GetTransferRequestsResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }
    }
}