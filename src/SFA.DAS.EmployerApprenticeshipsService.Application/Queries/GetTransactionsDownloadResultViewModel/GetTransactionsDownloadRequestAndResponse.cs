using System.Collections.Generic;
using System.Linq;
using MediatR;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
{
    public partial class
        GetTransactionsDownloadRequestAndResponse : IAsyncRequest<GetTransactionsDownloadRequestAndResponse>
    {
        public bool IsUnauthorized { get; set; }

        public byte[] FileDate { get; set; }

        public string MimeType { get; set; }

        public string FileExtension { get; set; }

        public DownloadFormatType DownloadFormat { get; set; }

        public long AccountId { get; set; }

        public string HashedId { get; set; }

        public string ExternalUserId { get; set; }

        public TransactionsDownloadStartDateMonthYearDateTime StartDate { get; set; } =
            new TransactionsDownloadStartDateMonthYearDateTime();

        public TransactionsDownloadEndDateMonthYearDateTime EndDate { get; set; } =
            new TransactionsDownloadEndDateMonthYearDateTime();

        public bool Valid => StartDate.Valid && EndDate.Valid;

        public bool HasSummaryErrors => EitherDateInFuture || FoundNoTransactions;

        public bool FoundNoTransactions => (Transactions != null && !Transactions.Any());

        public bool EitherDateInFuture => (EndDate.DateInFuture || StartDate.DateInFuture);

        public List<TransactionDownloadLine> Transactions { get; set; }

    }
}