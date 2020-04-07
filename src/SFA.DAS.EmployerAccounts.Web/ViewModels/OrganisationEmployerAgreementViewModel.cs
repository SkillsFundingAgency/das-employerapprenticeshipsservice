using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{

    public class OrganisationAgreementViewModelV1
    {
        public OrganisationAgreementViewModel OrganisationAgreementViewModel { get; set; }

        public bool OrganisationLookupPossible { get; set; }
    }
}