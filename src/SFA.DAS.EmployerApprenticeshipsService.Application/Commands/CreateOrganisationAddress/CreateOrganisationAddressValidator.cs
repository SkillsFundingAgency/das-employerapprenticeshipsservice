using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress
{
    public class CreateOrganisationAddressValidator : IValidator<CreateOrganisationAddressRequest>
    {
        //This is postcode regex found off stackoverflow that was an old version of the gov.uk office postcode
        //regex pattern. Given we only need to do basic validation on the postcode format this should be more
        //than enough to cover this. You can see the stackoverflow post here:
        //http://stackoverflow.com/questions/164979/uk-postcode-regex-comprehensive
        private const string PostcodeRegExPattern =
            "(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)" + 
            "|(([A-Z-[QVX]][0-9][A-HJKPSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY]" + 
            ")))) [0-9][A-Z-[CIKMOV]]{2})";

        public ValidationResult Validate(CreateOrganisationAddressRequest item)
        {
            var results = new ValidationResult();

            if (string.IsNullOrEmpty(item.AddressFirstLine))
            {
                results.ValidationDictionary.Add("AddressLine1", "Enter house number or name, building or street");
            }

            if (string.IsNullOrEmpty(item.TownOrCity))
            {
                results.ValidationDictionary.Add("TownOrCity", "Enter town or city");
            }

            if (string.IsNullOrEmpty(item.Postcode))
            {
                results.ValidationDictionary.Add("Postcode", "Enter a valid postcode");
            }

            if (!string.IsNullOrEmpty(item.Postcode) && !Regex.IsMatch(item.Postcode.ToUpper(), PostcodeRegExPattern))
            {
                results.ValidationDictionary.Add("Postcode", "Enter a valid postcode");
            }

            return results;
        }

        public Task<ValidationResult> ValidateAsync(CreateOrganisationAddressRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
