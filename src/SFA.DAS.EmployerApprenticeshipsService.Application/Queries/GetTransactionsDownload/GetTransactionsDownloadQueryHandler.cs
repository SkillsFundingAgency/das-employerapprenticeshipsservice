using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadQueryHandler : IAsyncRequestHandler<GetTransactionsDownloadQuery, GetTransactionsDownloadResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IHashingService _hashingService;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransactionFormatterFactory _transactionsFormatterFactory;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsDownloadQueryHandler(
            CurrentUser currentUser,
            IHashingService hashingService,
            IMembershipRepository membershipRepository,
            ITransactionFormatterFactory transactionsFormatterFactory,
            ITransactionRepository transactionRepository)
        {
            _currentUser = currentUser;
            _hashingService = hashingService;
            _membershipRepository = membershipRepository;
            _transactionsFormatterFactory = transactionsFormatterFactory;
            _transactionRepository = transactionRepository;
        }

        public async Task<GetTransactionsDownloadResponse> Handle(GetTransactionsDownloadQuery message)
        {
            var membership = await _membershipRepository.GetCaller(message.HashedAccountId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }
            
            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var toDate = new DateTime(message.EndDate.Year.Value, message.EndDate.Month.Value, 1).AddMonths(1);
            var transactions = await _transactionRepository.GetAllTransactionDetailsForAccountByDate(accountId, message.StartDate, toDate);

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