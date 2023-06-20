namespace SFA.DAS.EmployerAccounts.Models.PostcodeAnywhere;

/// <summary>
/// Postcode Anywhere address DTO
/// </summary>
internal class PcaAddress
{
    public string Udprn { get; set; }
    public string Company { get; set; }
    public string Department { get; set; }
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string Line3 { get; set; }
    public string Line4 { get; set; }
    public string Line5 { get; set; }
    public string PostTown { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
    public int Mailsort { get; set; }
    public string Barcode { get; set; }
    public string Type { get; set; }
    public string DeliveryPointSuffix { get; set; }
    public string SubBuilding { get; set; }
    public string BuildingName { get; set; }
    public string BuildingNumber { get; set; }
    public string PrimaryStreet { get; set; }
    public string SecondaryStreet { get; set; }
    public string DoubleDependentLocality { get; set; }
    public string DependentLocality { get; set; }
    public string PoBox { get; set; }
    public string PrimaryStreetName { get; set; }
    public string PrimaryStreetType { get; set; }
    public string SecondaryStreetName { get; set; }
    public string SecondaryStreetType { get; set; }
    public string CountryName { get; set; }
    public string CountryISO2 { get; set; }
    public string CountryISO3 { get; set; }
}