using AutoMapper;
using SFA.DAS.EAS.Domain.Models.ExpiredFunds;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Domain.Models.Transfers;

namespace SFA.DAS.EAS.Application.Mappings;

public class TransactionMappings : Profile
{
    public TransactionMappings()
    {
        CreateMap<TransactionEntity, TransactionLine>();
        CreateMap<TransactionEntity, PaymentTransactionLine>();
        CreateMap<TransactionEntity, LevyDeclarationTransactionLine>();
        CreateMap<TransactionEntity, TransferTransactionLine>();
        CreateMap<TransactionEntity, ExpiredFundTransactionLine>();
    }
}
