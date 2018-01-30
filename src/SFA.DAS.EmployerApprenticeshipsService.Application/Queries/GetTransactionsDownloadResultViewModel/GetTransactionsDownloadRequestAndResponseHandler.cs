using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
{
    public class GetTransactionsDownloadRequestAndResponseHandler : IAsyncRequestHandler<GetTransactionsDownloadRequestAndResponse, GetTransactionsDownloadRequestAndResponse>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IValidator<GetTransactionsDownloadRequestAndResponse> _validator;
        private readonly ITransactionFormatterFactory _transactionsFormatterFactory;
        private readonly IMembershipRepository _membershipRepository;

        public GetTransactionsDownloadRequestAndResponseHandler(ITransactionRepository transactionRepository,
            IValidator<GetTransactionsDownloadRequestAndResponse> validator,
            ITransactionFormatterFactory transactionsFormatterFactory, IMembershipRepository membershipRepository)
        {
            _transactionRepository = transactionRepository;
            _validator = validator;
            _transactionsFormatterFactory = transactionsFormatterFactory;
            _membershipRepository = membershipRepository;
        }

        public async Task<GetTransactionsDownloadRequestAndResponse> Handle(GetTransactionsDownloadRequestAndResponse message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            if (result.IsUnauthorized)
            {
                message.IsUnauthorized = true;
                return message;
            }

            var caller = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);
            if (caller == null)
            {
                message.IsUnauthorized = true;
                return message;
            }

            message.Transactions =
                await _transactionRepository.GetAllTransactionDetailsForAccountByDate(
                    message.AccountId,
                    message.StartDate.ToDateTime(),
                    message.EndDate.ToDateTime());

            var fileFormatter = _transactionsFormatterFactory.GetTransactionsFormatterByType(message.DownloadFormat);
            message.FileDate = fileFormatter.GetFileData(message.Transactions);
            message.FileExtension = fileFormatter.FileExtension;
            message.MimeType = fileFormatter.MimeType;
            message.IsUnauthorized = false;
            return message;
        }
    }
}