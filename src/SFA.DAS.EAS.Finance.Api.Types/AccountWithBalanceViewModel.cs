//using SFA.DAS.Common.Domain.Types;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SFA.DAS.EAS.Finance.Api.Types
//{
//    public class AccountWithBalanceViewModel
//    {
//        public string AccountName { get; set; }

//        public string AccountHashId { get; set; }

//        public string PublicAccountHashId { get; set; }

//        public long AccountId { get; set; }

//        public decimal Balance { get; set; }

//        [Obsolete("This property is now being replaced by RemainingTransferAllowance")]
//        public decimal TransferAllowance
//        {
//            get => RemainingTransferAllowance;
//            set => RemainingTransferAllowance = value;
//        }

//        public decimal RemainingTransferAllowance { get; set; }

//        public decimal StartingTransferAllowance { get; set; }

//        public string Href { get; set; }

//        [Obsolete("Use AllowedOnService instead")]
//        public bool IsLevyPayer { get; set; }
//        public bool IsAllowedPaymentOnService { get; set; }
//        public AccountAgreementType AccountAgreementType { get; set; }
//        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }

//    }   
//}
