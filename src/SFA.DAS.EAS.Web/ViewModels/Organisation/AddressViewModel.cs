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
            return $"{AddressFirstLine}, {AddressSecondLine}, {TownOrCity}, {County} {Postcode}";
        }
    }
}