﻿using System;

namespace SFA.DAS.EmployerFinance.Models.Payments
{
    public class PeriodEnd
    {
        public int Id { get; set; }

        public string PeriodEndId { get; set; }

        public int CalendarPeriodMonth { get; set; }

        public int CalendarPeriodYear { get; set; }

        public DateTime? AccountDataValidAt { get; set; }

        public DateTime? CommitmentDataValidAt { get; set; }

        public DateTime CompletionDateTime { get; set; }

        public string PaymentsForPeriod { get; set; }
    }
}
