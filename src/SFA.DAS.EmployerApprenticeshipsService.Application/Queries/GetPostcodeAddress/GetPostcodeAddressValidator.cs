using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressValidator : IValidator<GetPostcodeAddressRequest>
    {
        //This is postcode regex found off stackoverflow that was an old version of the gov.uk office postcode
        //regex pattern. Given we only need to do basic validation on the postcode format this should be more
        //than enough to cover this. You can see the stackoverflow post here:
        //http://stackoverflow.com/questions/164979/uk-postcode-regex-comprehensive
        private const string PostcodeRegExPattern =
            "(GIR 0AA)|(GIR0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)" +
            "|(([A-Z-[QVX]][0-9][A-HJKPSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY]" +
            "))))[ ]?[0-9][A-Z-[CIKMOV]]{2})";

        private const short MaxPostCodeLength = 8;


        public ValidationResult Validate(GetPostcodeAddressRequest item)
        {
            var results = new ValidationResult();

            if (string.IsNullOrEmpty(item.Postcode))
            {
                results.ValidationDictionary.Add(nameof(item.Postcode), "Enter a valid postcode");
            }

            if (!results.ValidationDictionary.ContainsKey(nameof(item.Postcode)) &&
             (!string.IsNullOrEmpty(item.Postcode) && item.Postcode.Trim().Length > MaxPostCodeLength))
                results.ValidationDictionary.Add(nameof(item.Postcode), "Enter a valid postcode");

            if (!results.ValidationDictionary.ContainsKey(nameof(item.Postcode)) &&
                (!string.IsNullOrEmpty(item.Postcode) &&
                 !Regex.IsMatch(item.Postcode.ToUpper(), PostcodeRegExPattern)))
                results.ValidationDictionary.Add(nameof(item.Postcode), "Enter a valid postcode");

            return results;
        }

        public Task<ValidationResult> ValidateAsync(GetPostcodeAddressRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
