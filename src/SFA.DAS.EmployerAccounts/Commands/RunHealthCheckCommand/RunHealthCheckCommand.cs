﻿using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand
{
    public class RunHealthCheckCommand : IAsyncRequest
    {
        [IgnoreMap]
        [Required]
        public Guid? UserRef { get; set; }
    }
}