using AutoMapper;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Models.Transfers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Mapping.Profiles
{
    public class TransactionMappings : Profile
    {
        public TransactionMappings()
        {
            CreateMap<TransactionEntity, TransactionLine>();
            CreateMap<TransactionEntity, PaymentTransactionLine>();
            CreateMap<TransactionEntity, LevyDeclarationTransactionLine>();
            CreateMap<TransactionEntity, TransferTransactionLine>();

        }
    }
}
