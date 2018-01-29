using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel
{
    public class GetTransactionsDownloadRequestAndResponseValidator : IValidator<GetTransactionsDownloadRequestAndResponse>
    {
        public MembershipView Caller;

        public ValidationResult Validate(GetTransactionsDownloadRequestAndResponse item)
        {
            var result = new ValidationResult();

            if (item.AccountId == 0)
            {
                result.AddError(nameof(item.AccountId), "AccountId has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetTransactionsDownloadRequestAndResponse item)
        {
            return Task.Run(() => Validate(item));
        }
    }
}