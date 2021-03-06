﻿using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AgreementViewModel
    {
        public long Id { get; set; }
        public DateTime? SignedDate { get; set; }
        public string SignedByName { get; set; }
        public EmployerAgreementStatus Status { get; set; }
        public int TemplateVersionNumber { get; set; }
        public AgreementType AgreementType { get; set; }
    }
}