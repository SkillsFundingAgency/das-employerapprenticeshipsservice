﻿using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class SelectOrganisationAddressViewModel : OrganisationViewModelBase
    {
        public string Postcode { get; set; }

        public string PostcodeError => GetErrorMessage(nameof(Postcode));

        public ICollection<AddressViewModel> Addresses { get; set; }
    }
}