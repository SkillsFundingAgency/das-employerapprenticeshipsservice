﻿using System;

namespace SFA.DAS.EmployerAccounts.Dtos
{
    public class AgreementTemplateDto
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string PartialViewName { get; set; }
        public int VersionNumber { get; set; }
    }
}
