using MediatR;
using SFA.DAS.EmployerFinance.Attributes;
using SFA.DAS.EmployerFinance.Messages;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;

namespace SFA.DAS.EmployerFinance.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadQuery : IAuthorizationContextModel, IAsyncRequest<GetTransactionsDownloadResponse>
    {
        [IgnoreMap]
        [Required]
        public long AccountId { get; set; }

        [Display(Name = "Start date")]
        [Required]
        [Month, Year, Date]
        public MonthYear StartDate { get; set; }

        [Display(Name = "End date")]
        [Required]
        [Month, Year, Date]
        public MonthYear EndDate { get; set; }

        [Required]
        public DownloadFormatType? DownloadFormat { get; set; }
    }
}