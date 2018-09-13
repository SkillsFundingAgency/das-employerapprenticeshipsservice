using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadQueryHandler : IAsyncRequestHandler<GetTransactionsDownloadQuery, GetTransactionsDownloadResponse>
    {
        private readonly ITransactionFormatterFactory _transactionsFormatterFactory;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsDownloadQueryHandler(ITransactionFormatterFactory transactionsFormatterFactory, ITransactionRepository transactionRepository)
        {
            _transactionsFormatterFactory = transactionsFormatterFactory;
            _transactionRepository = transactionRepository;
        }

        public async Task<GetTransactionsDownloadResponse> Handle(GetTransactionsDownloadQuery message)
        {
            var endDate = message.EndDate.ToDate();
            var endDateBeginningOfNextMonth = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1);
            var transactions = await _transactionRepository.GetAllTransactionDetailsForAccountByDate(message.AccountId.Value, message.StartDate, endDateBeginningOfNextMonth);

            if (!transactions.Any())
            {
                throw new ValidationException("There are no transactions in the date range");
            }

            var fileFormatter = _transactionsFormatterFactory.GetTransactionsFormatterByType(message.DownloadFormat.Value);

            return new GetTransactionsDownloadResponse
            {
                FileData = fileFormatter.GetFileData(transactions),
                FileExtension = fileFormatter.FileExtension,
                MimeType = fileFormatter.MimeType
            };
        }
    }
}