﻿using System.Collections.Generic;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class EmployerAgreementListViewModel
    {
        public long AccountId { get; set; }
        public GetAccountEmployerAgreementsResponse EmployerAgreementsData { get; set; }
        public string HashedAccountId { get; set; }
    }
}