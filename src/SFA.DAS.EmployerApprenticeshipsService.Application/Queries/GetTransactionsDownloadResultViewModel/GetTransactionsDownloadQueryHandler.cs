using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
{
    public class GetTransactionsDownloadQueryHandler : IAsyncRequestHandler<GetTransactionsDownloadQuery, GetTransactionsDownloadResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionFormatterFactory _transactionsFormatterFactory;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IHashingService _hashingService;

        public GetTransactionsDownloadQueryHandler(
            CurrentUser currentUser,
            ITransactionRepository transactionRepository,
            ITransactionFormatterFactory transactionsFormatterFactory, 
            IMembershipRepository membershipRepository,
            IHashingService hashingService)
        {
            _currentUser = currentUser;
            _transactionRepository = transactionRepository;
            _transactionsFormatterFactory = transactionsFormatterFactory;
            _membershipRepository = membershipRepository;
            _hashingService = hashingService;
        }

        public async Task<GetTransactionsDownloadResponse> Handle(GetTransactionsDownloadQuery message)
        {
            var membership = await _membershipRepository.GetCaller(message.AccountHashedId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

            var validationResult = new ValidationResult();

            if (message.StartDate.DateInFuture)
            {
                validationResult.AddError(nameof(message.StartDate),
                    $"The latest date you can enter is {message.StartDate.MaximumDate.ToString("MM yyyy")}");
            }
            else if (!message.StartDate.Valid)
            {
                validationResult.AddError(nameof(message.StartDate), "Enter a different start date");
            }

            if (message.EndDate.DateInFuture)
            {
                validationResult.AddError(nameof(message.EndDate),
                    $"The latest date you can enter is {message.EndDate.MaximumDate.ToString("MM yyyy")}");
            }
            else if (!message.EndDate.Valid)
            {
                validationResult.AddError(nameof(message.EndDate), "Enter a different end date");
            }

            if (!validationResult.IsValid())
            {
                return new GetTransactionsDownloadResponse
                {
                    ValidationResult = validationResult
                };
            }

            var accountId = _hashingService.DecodeValue(message.AccountHashedId);
            var transactionsTask = _transactionRepository.GetAllTransactionDetailsForAccountByDate(
                accountId,
                message.StartDate.ToDateTime(),
                message.EndDate.ToDateTime());

            var transactions = await transactionsTask;

            if (transactions == null || !transactions.Any())
            {
                validationResult.AddError(nameof(message.AccountHashedId), "There are no transactions in the date range");
            }

            var fileFormatter = _transactionsFormatterFactory.GetTransactionsFormatterByType(message.DownloadFormat);

            return new GetTransactionsDownloadResponse
            {
                FileDate = fileFormatter.GetFileData(transactions),
                FileExtension = fileFormatter.FileExtension,
                MimeType = fileFormatter.MimeType,
                ValidationResult = validationResult
            };
        }
    }
}