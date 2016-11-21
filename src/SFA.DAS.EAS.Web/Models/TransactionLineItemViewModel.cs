﻿using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Web.Models
{
    public class TransactionLineItemViewModel
    {
        public List<LevyDeclarationTransactionLine> LineItem { get; set; }
        public decimal TotalAmount { get; set; }
    }
}