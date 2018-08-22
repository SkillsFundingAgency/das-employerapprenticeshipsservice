﻿using System;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.DtosX
{
    public class AgreementDto
    {
        public long Id { get; set; }
        public AccountDto Account { get; set; }
        public long AccountId { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string HashedAccountId { get; set; }
        public string HashedAgreementId { get; set; }
        public LegalEntityDto LegalEntity { get; set; }
        public long LegalEntityId { get; set; }
        public long? SignedById { get; set; }
        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public EmployerAgreementStatus StatusId { get; set; }
        public AgreementTemplateDto Template { get; set; }
        public int TemplateId { get; set; }
    }
}