using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity
{
    public class AddPayeToNewLegalEntityCommandValidator : IValidator<AddPayeToNewLegalEntityCommand>
    {
        private readonly IMembershipRepository _membershipRepository;

        public AddPayeToNewLegalEntityCommandValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(AddPayeToNewLegalEntityCommand item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(AddPayeToNewLegalEntityCommand item)
        {
            var validationResult = new ValidationResult();

            CheckFieldsArePopulated(item, validationResult);

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(item.AccountId, item.ExternalUserId);

                if (member == null)
                {
                    validationResult.AddError(nameof(member),"Unauthorised: User not connected to account");
                }
                else
                {
                    if (member.RoleId != (short) Role.Owner)
                    {
                        validationResult.AddError(nameof(member), "Unauthorised: User is not an owner");
                    }
                }
            }

            return validationResult;
        }

        private static void CheckFieldsArePopulated(AddPayeToNewLegalEntityCommand item, ValidationResult validationResult)
        {
            if (string.IsNullOrEmpty(item.AccessToken))
            {
                validationResult.AddError(nameof(item.AccessToken), "AccessToken has not been supplied");
            }
            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId), "AccountId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.Empref))
            {
                validationResult.AddError(nameof(item.Empref), "Empref has not been supplied");
            }
            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.LegalEntityAddress))
            {
                validationResult.AddError(nameof(item.LegalEntityAddress), "LegalEntityAddress has not been supplied");
            }
            if (string.IsNullOrEmpty(item.LegalEntityCode))
            {
                validationResult.AddError(nameof(item.LegalEntityCode), "LegalEntityCode has not been supplied");
            }
            if (item.LegalEntityDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.LegalEntityDate), "LegalEntityDate has not been supplied");
            }
            if (string.IsNullOrEmpty(item.LegalEntityName))
            {
                validationResult.AddError(nameof(item.LegalEntityName), "LegalEntityName has not been supplied");
            }
            if (string.IsNullOrEmpty(item.RefreshToken))
            {
                validationResult.AddError(nameof(item.RefreshToken), "RefreshToken has not been supplied");
            }
        }
    }
}