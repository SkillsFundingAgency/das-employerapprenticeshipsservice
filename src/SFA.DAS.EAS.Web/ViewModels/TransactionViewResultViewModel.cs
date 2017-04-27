using System;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class TransactionViewResultViewModel
    {
        public Account Account { get; set; }
        public TransactionViewModel Model { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public bool IsLatestMonth => DateTime.Now.Year == Year && DateTime.Now.Month == Month;
    }
}