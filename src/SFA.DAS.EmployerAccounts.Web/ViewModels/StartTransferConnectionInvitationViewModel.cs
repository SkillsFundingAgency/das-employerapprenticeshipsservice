﻿using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class StartTransferConnectionInvitationViewModel : IAuthorizationContextModel
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [IgnoreMap]
        [Required]
        public Guid UserRef { get; set; }

        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(EmployerAccounts.Constants.AccountHashedIdRegex, ErrorMessage = "You must enter a valid account ID")]
        public string ReceiverAccountPublicHashedId { get; set; }
    }
}