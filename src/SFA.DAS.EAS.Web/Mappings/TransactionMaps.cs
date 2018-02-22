using AutoMapper;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownload;
using SFA.DAS.EAS.Web.ViewModels.Transactions;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class TransactionMaps : Profile
    {
        public TransactionMaps()
        {
            CreateMap<TransactionDownloadViewModel, GetTransactionsDownloadQuery>()
                .ForMember(m => m.StartDate, o => o.Ignore())
                .ForMember(m => m.EndDate, o => o.Ignore())
                .ForMember(m => m.DownloadFormat, o => o.Ignore());
        }
    }
}