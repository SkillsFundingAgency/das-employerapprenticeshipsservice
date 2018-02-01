using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
{
    public class GetTransactionsDownloadQuery : IAsyncRequest<GetTransactionsDownloadResponse>
    {
        [Required]
        public TransactionsDownloadStartDateMonthYearDateTime StartDate { get; set; } =
            new TransactionsDownloadStartDateMonthYearDateTime();

        [Required]
        public TransactionsDownloadEndDateMonthYearDateTime EndDate { get; set; } =
            new TransactionsDownloadEndDateMonthYearDateTime();

        public DownloadFormatType DownloadFormat { get; set; }

        [Required(ErrorMessage = "You must enter a valid account ID")]
        [RegularExpression(@"^[A-Za-z\d]{6,6}$", ErrorMessage = "You must enter a valid account ID")]
        public string AccountHashedId { get; set; }

        public bool HasSummaryErrors { get; set; }
    }
}