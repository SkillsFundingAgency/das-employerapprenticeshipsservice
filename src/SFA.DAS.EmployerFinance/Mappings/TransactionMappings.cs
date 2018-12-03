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
            CreateMap<TransactionEntity, TransactionLine>()
                .ForMember(d => d.Description, o => o.Ignore())
                .ForMember(d => d.SubTransactions, o => o.Ignore())
                .ForMember(d => d.PayrollDate, o => o.Ignore());

            CreateMap<TransactionEntity, PaymentTransactionLine>()
                .ForMember(d => d.PaymentId, o => o.Ignore())
                .ForMember(d => d.Description, o => o.Ignore())
                .ForMember(d => d.SubTransactions, o => o.Ignore())
                .ForMember(d => d.PayrollDate, o => o.Ignore());

            CreateMap<TransactionEntity, LevyDeclarationTransactionLine>()
                .ForMember(d => d.PayeSchemeName, o => o.Ignore())
                .ForMember(d => d.LineTotal, o => o.Ignore())
                .ForMember(d => d.Description, o => o.Ignore())
                .ForMember(d => d.SubTransactions, o => o.Ignore())
                .ForMember(d => d.PayrollDate, o => o.Ignore());

            CreateMap<TransactionEntity, TransferTransactionLine>()
                .ForMember(d => d.ReceiverAccountPublicHashedId, o => o.Ignore())
                .ForMember(d => d.SenderAccountPublicHashedId, o => o.Ignore())
                .ForMember(d => d.Description, o => o.Ignore())
                .ForMember(d => d.SubTransactions, o => o.Ignore())
                .ForMember(d => d.PayrollDate, o => o.Ignore());
        }
    }
}