using AutoMapper;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Models.Transfers;

namespace SFA.DAS.EmployerFinance.Mappings
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
