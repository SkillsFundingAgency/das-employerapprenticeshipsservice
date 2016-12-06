using System;
using FluentValidation;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Validators
{
    using System.Text.RegularExpressions;

    public class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            var currentYear = DateTime.Now.Year;

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Enter a first name");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Enter a last name");

            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Enter a valid unique learner number");

            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Enter the total agreed training cost");
            RuleFor(x => x.StartMonth).InclusiveBetween(1, 12).WithMessage("Enter a valid month for training start");
            RuleFor(x => x.StartYear).InclusiveBetween(currentYear, 9999).WithMessage("Enter a valid year for training start");
            RuleFor(x => x.EndMonth).InclusiveBetween(1, 12).WithMessage("Enter a valid month for training finish");
            RuleFor(x => x.EndYear).InclusiveBetween(currentYear, 9999).WithMessage("Enter a valid year for training finish");

            RuleFor(x => x.NINumber)
                .Matches(@"^[abceghj-prstw-z][abceghj-nprstw-z]\d{6}[abcd]$", RegexOptions.IgnoreCase)
                .WithMessage("Enter a valid national insurance number");

            RuleFor(x => x.DateOfBirthDay)
                .InclusiveBetween(1, 31)
                .WithMessage("Enter a valid day for date of birth");
            RuleFor(x => x.DateOfBirthMonth)
                .InclusiveBetween(1, 12)
                .WithMessage("Enter a valid month for date of birth");
            RuleFor(x => x.DateOfBirthYear)
                .InclusiveBetween(1900, currentYear)
                .WithMessage("Enter a valid year for date of birth");
        }
    }
}