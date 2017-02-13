﻿using System.Collections.Generic;
using FluentValidation.Attributes;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.ViewModels
{
    [Validator(typeof(DeleteCohortConfirmationViewModelValidator))]
    public sealed class DeleteCommitmentViewModel{

        public string HashedAccountId { get; set; }

        public string HashedCommitmentId { get; set; }

        public string ProviderName { get; set; }

        public int NumberOfApprenticeships { get; set; }

        public List<string> ProgrammeSummaries { get; set; }

        public bool? DeleteConfirmed { get; set; }
    }
}