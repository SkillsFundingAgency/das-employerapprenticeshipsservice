using MediatR;
using SFA.DAS.EmployerFinance.Attributes;
using SFA.DAS.EmployerFinance.Messages;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;

namespace SFA.DAS.EmployerFinance.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadQuery : MembershipMessage, IAsyncRequest<GetTransactionsDownloadResponse>
    {
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