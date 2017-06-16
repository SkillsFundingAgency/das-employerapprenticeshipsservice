﻿using System;
using System.Collections.Generic;

using Microsoft.SqlServer.Server;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class ApprenticeshipDetailsViewModel
    {
        public string HashedApprenticeshipId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TrainingName { get; set; }

        public decimal? Cost { get; set; }

        public string Status { get; set; }

        public string ProviderName { get; set; }

        public PendingChanges PendingChanges { get; set; }
        
        public bool CanEditStatus { get;  set; }

        public IEnumerable<string> Alerts { get; set; }

        public string EmployerReference { get; set; }

        public string CohortReference { get; set; }

        public bool EnableEdit { get; set; }

        public bool PendingDataLockRestart { get; set; }

        public bool PendingDataLockChange { get; set; }
    }

    public enum PendingChanges
    {
        None = 0,
        ReadyForApproval = 1,
        WaitingForApproval = 2
    }

    public enum TriageStatusViewModel
    {
        Unknown = 0,
        Change = 1,
        Restart = 2,
        FixIlr = 3
    }
}