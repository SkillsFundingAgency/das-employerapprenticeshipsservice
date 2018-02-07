using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadQueryHandler : IAsyncRequestHandler<GetTransactionsDownloadQuery, GetTransactionsDownloadResponse>
    {
        private readonly IHashingService _hashingService;
        private readonly ITransactionFormatterFactory _transactionsFormatterFactory;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsDownloadQueryHandler(
            IHashingService hashingService,
            ITransactionFormatterFactory transactionsFormatterFactory,
            ITransactionRepository transactionRepository)
        {
            _hashingService = hashingService;
            _transactionsFormatterFactory = transactionsFormatterFactory;
            _transactionRepository = transactionRepository;
        }

        public async Task<GetTransactionsDownloadResponse> Handle(GetTransactionsDownloadQuery message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashedId);
            var endDate = message.EndDate.ToDate();
            var endDateBeginningOfNextMonth = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1);
            var transactions = await _transactionRepository.GetAllTransactionDetailsForAccountByDate(accountId, message.StartDate, endDateBeginningOfNextMonth);

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