using System;
using System.Text.RegularExpressions;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandValidator : IValidator<CreateInvitationCommand>
    {
        public ValidationResult Validate(CreateInvitationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
                validationResult.AddError("AccountId", "No AccountId supplied");

            if (string.IsNullOrWhiteSpace(item.Email))
                validationResult.AddError("Email", "No Email supplied");
            else
            {
                if (!IsValidEmailFormat(item.Email))
                {
                    validationResult.AddError("Email", "Email is not valid format");
                }
            }

            if (string.IsNullOrWhiteSpace(item.Name))
                validationResult.AddError("Name", "No Name supplied");

            if (item.RoleId == 0)
                validationResult.AddError("RoleId", "No RoleId supplied");

            return validationResult;
        }

        private bool IsValidEmailFormat(string email)
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
    }
}