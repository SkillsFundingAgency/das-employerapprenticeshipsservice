using AutoMapper;
using SFA.DAS.EAS.Domain.Entities.Transaction;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Infrastructure.Mapping.Profiles
{
    public class TransactionMappings : Profile
    {
        public TransactionMappings()
        {
            CreateMap<TransactionEntity, TransactionLine>();
            CreateMap<TransactionEntity, PaymentTransactionLine>();
            CreateMap<TransactionEntity, LevyDeclarationTransactionLine>();
        }
    }
}
