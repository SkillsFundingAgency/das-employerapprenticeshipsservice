using System.Text;
using Address = SFA.DAS.ReferenceData.Types.DTO.Address;

namespace SFA.DAS.EmployerAccounts.Extensions;

public static class AddressExtensions
{
    public static string FormatAddress(this SFA.DAS.EmployerAccounts.Models.Organisation.Address address)
    {
        var sb = new StringBuilder();

        AddIfNotNull(address.Line1, sb);
        AddIfNotNull(address.Line2, sb);
        AddIfNotNull(address.Line3, sb);
        AddIfNotNull(address.Line4, sb);
        AddIfNotNull(address.Line5, sb);
        AddIfNotNull(address.Postcode, sb);

        return sb.ToString();
    }

    public static string FormatAddress(this Address address)
    {
        var sb = new StringBuilder();

        AddIfNotNull(address.Line1, sb);
        AddIfNotNull(address.Line2, sb);
        AddIfNotNull(address.Line3, sb);
        AddIfNotNull(address.Line4, sb);
        AddIfNotNull(address.Line5, sb);
        AddIfNotNull(address.Postcode, sb);

        return sb.ToString();
    }

    private static void AddIfNotNull(string s, StringBuilder sb)
    {
        if (!string.IsNullOrWhiteSpace(s))
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }

            sb.Append(s);
        }
    }
}