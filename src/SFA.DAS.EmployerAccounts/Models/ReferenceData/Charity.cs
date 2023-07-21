namespace SFA.DAS.EmployerAccounts.Models.ReferenceData;

public class Charity
{
    public int RegistrationNumber { get; set; }
    public string Name { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string Address4 { get; set; }
    public string Address5 { get; set; }
    public string PostCode { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsRemoved { get; set; }

    public string FormattedAddress
    {
        get
        {
            var addressLines = new List<string>();
            if (!string.IsNullOrWhiteSpace(Address1)) { addressLines.Add(Address1); }
            if (!string.IsNullOrWhiteSpace(Address2)) { addressLines.Add(Address2); }
            if (!string.IsNullOrWhiteSpace(Address3)) { addressLines.Add(Address3); }
            if (!string.IsNullOrWhiteSpace(Address4)) { addressLines.Add(Address4); }
            if (!string.IsNullOrWhiteSpace(Address5)) { addressLines.Add(Address5); }
            if (!string.IsNullOrWhiteSpace(PostCode)) { addressLines.Add(PostCode); }
            return string.Join(", ", addressLines.ToArray());
        }
    }
}