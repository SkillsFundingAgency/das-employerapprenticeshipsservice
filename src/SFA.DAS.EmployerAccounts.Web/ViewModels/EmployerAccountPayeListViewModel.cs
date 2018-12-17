﻿using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class EmployerAccountPayeListViewModel
    {
        public string HashedId { get; set; }
                    
        public List<PayeView> PayeSchemes { get; set; }
    }
}