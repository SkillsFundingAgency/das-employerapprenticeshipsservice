using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadQueryHandler : IAsyncRequestHandler<GetTransactionsDownloadQuery, GetTransactionsDownloadResponse>
    {
        private readonly ITransactionFormatterFactory _transactionsFormatterFactory;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountApiClient _accountApiClient;

        public GetTransactionsDownloadQueryHandler(
            ITransactionFormatterFactory transactionsFormatterFactory, 
            ITransactionRepository transactionRepository,
            IAccountApiClient accountApiClient)
        {
            _transactionsFormatterFactory = transactionsFormatterFactory;
            _transactionRepository = transactionRepository;
            _accountApiClient = accountApiClient;
        }

        public async Task<GetTransactionsDownloadResponse> Handle(GetTransactionsDownloadQuery message)
        {
            var endDate = message.EndDate.ToDate();
            var endDateBeginningOfNextMonth = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1);
            var transactions = await _transactionRepository.GetAllTransactionDetailsForAccountByDate(message.AccountId, message.StartDate, endDateBeginningOfNextMonth);
                       
            if (!transactions.Any())
            {
                throw new ValidationException("There are no transactions in the date range");
            }

            var accountResponse = await _accountApiClient.GetAccount(message.AccountId);
            var apprenticeshipEmployerTypeEnum = (ApprenticeshipEmployerType)Enum.Parse(typeof(ApprenticeshipEmployerType), accountResponse.ApprenticeshipEmployerType, true);

            foreach (var transaction in transactions)
            {
                GenerateTransactionDescription(transaction);
            }

            var fileFormatter = _transactionsFormatterFactory.GetTransactionsFormatterByType(
                message.DownloadFormat.Value,
                apprenticeshipEmployerTypeEnum);

            return new GetTransactionsDownloadResponse
            {
                FileData = fileFormatter.GetFileData(transactions),
                FileExtension = fileFormatter.FileExtension,
                MimeType = fileFormatter.MimeType
            };
        }

        private void GenerateTransactionDescription(TransactionDownloadLine transaction)
        {
            transaction.Description = transaction.TransactionType;

            if (transaction.TransactionType.Equals("Payment", StringComparison.OrdinalIgnoreCase))
            {
                transaction.Description = transaction.TrainingProviderFormatted;
            }
            else if (transaction.TransactionType.Equals("Transfer", StringComparison.OrdinalIgnoreCase))
            {
                if (transaction.TransferSenderAccountId.Equals(transaction.AccountId))
                {
                    transaction.Description = $"Transfer sent to {transaction.TransferReceiverAccountName}";
                }
                else
                {
                    transaction.Description = $"Transfer received from {transaction.TransferSenderAccountName}";
                }
            }
        }
    }
}