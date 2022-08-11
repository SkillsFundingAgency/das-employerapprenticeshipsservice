//using System;
//using System.Threading.Tasks;
//using SFA.DAS.Validation;

//namespace SFA.DAS.EAS.Application.Queries.GetLevyDeclaration
//{
//    public class GetLevyDeclarationValidator : IValidator<GetLevyDeclarationRequest>
//    {
//        public ValidationResult Validate(GetLevyDeclarationRequest item)
//        {
//            var result = new ValidationResult();

//            if (string.IsNullOrEmpty(item.HashedAccountId))
//            {
//                result.AddError(nameof(item.HashedAccountId),"HashedAccountId has not been supplied");
//            }

//            return result;
//        }

//        public Task<ValidationResult> ValidateAsync(GetLevyDeclarationRequest item)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}