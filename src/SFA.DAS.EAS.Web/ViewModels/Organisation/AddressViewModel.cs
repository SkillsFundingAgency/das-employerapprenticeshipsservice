using System.Text;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class AddressViewModel : ViewModelBase
    {
        public string AddressFirstLine { get; set; }
        public string AddressSecondLine { get; set; }
        public string TownOrCity { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }

        public string AddressFirstLineError => GetErrorMessage(nameof(AddressFirstLine));
        public string TownOrCityError => GetErrorMessage(nameof(TownOrCity));
        public string PostcodeError => GetErrorMessage(nameof(Postcode));

        public override string ToString()
        {
            var addressBuilder = new StringBuilder();

            addressBuilder.Append(AddressFirstLine);

            if (!string.IsNullOrEmpty(AddressSecondLine))
            {
                addressBuilder.Append($", {AddressSecondLine}");
            }

            if (!string.IsNullOrEmpty(TownOrCity))
            {
                addressBuilder.Append($", {TownOrCity}");
            }

            if (!string.IsNullOrEmpty(County))
            {
                addressBuilder.Append($", {County}");
            }
            
            addressBuilder.Append($" {Postcode}");

            return addressBuilder.ToString();
        }
    }
}