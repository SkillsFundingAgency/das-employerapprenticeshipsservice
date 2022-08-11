//using System.Threading.Tasks;
//using SFA.DAS.Validation;
//using SFA.DAS.EAS.Domain.Data.Repositories;

//namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions
//{
//    public class GetEmployerAccountTransactionsValidator : IValidator<GetEmployerAccountTransactionsQuery>
//    {
//        private readonly IMembershipRepository _membershipRepository;

//        public GetEmployerAccountTransactionsValidator(IMembershipRepository membershipRepository)
//        {
//            _membershipRepository = membershipRepository;
//        }

//        public ValidationResult Validate(GetEmployerAccountTransactionsQuery item)
//        {
//            throw new System.NotImplementedException();
//        }

//        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountTransactionsQuery item)
//        {
//            var result = new ValidationResult();

//            if (string.IsNullOrEmpty(item.HashedAccountId))
//            {
//                result.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
//            }

//            if (result.IsValid() && !string.IsNullOrEmpty(item.ExternalUserId))
//            {
//                var caller = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
//                if (caller == null)
//                {
//                    result.IsUnauthorized = true;
//                }
//            }

//            return result;
//        }
//    }
//}