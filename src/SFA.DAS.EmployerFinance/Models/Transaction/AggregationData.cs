using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Models.Transaction
{
    public class AggregationData
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public decimal Balance { get; set; }
        public TransactionLine[] TransactionLines { get; set; } //TODO: Check this TransactionLine[]  or ICollection<TransactionLine>
        //public ICollection<TransactionLine> TransactionLines { get; set; }
    }

    //TODO : Check this class below Model  from Accounts , above the existing model in Finance Check TransactionLine[] or ICollection<TransactionLine>
    //public class AggregationData
    //{
    //    public long AccountId { get; set; }
    //    public string HashedAccountId { get; set; }
    //    public ICollection<TransactionLine> TransactionLines { get; set; }
    //}
}