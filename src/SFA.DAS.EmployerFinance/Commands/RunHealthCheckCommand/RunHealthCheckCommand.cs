using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommand : IAuthorizationContextModel, IAsyncRequest
    {
        [IgnoreMap]
        [Required]
        public Guid? UserRef { get; set; }
    }
}