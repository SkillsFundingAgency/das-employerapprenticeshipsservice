using System.Text;
using HashidsNet;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public class PayRefHashingService : IPayRefHashingService
{
    private readonly Hashids _hashIds;

    public PayRefHashingService(string allowedCharacters, string hashstring)
    {
        if (string.IsNullOrEmpty(allowedCharacters))
        {
            throw new ArgumentException("Value cannot be null", nameof(allowedCharacters));
        }

        if (string.IsNullOrEmpty(hashstring))
        {
            throw new ArgumentException("Value cannot be null", nameof(hashstring));
        }

        _hashIds = new Hashids(hashstring, 6, allowedCharacters);
    }

    public string HashValue(string id)
    {
        string hex = StringToHex(id).Replace("-", "");
        return _hashIds.EncodeHex(hex);
    }

    public string DecodeValueToString(string id)
    {
        ValidateInput(id);
        string hex = _hashIds.DecodeHex(id);
        return FromHexToString(hex);
    }

    private static string StringToHex(string stringValue)
    {
        return BitConverter.ToString(new ASCIIEncoding().GetBytes(stringValue));
    }

    private static string FromHexToString(string hex)
    {
        byte[] array = new byte[hex.Length / 2];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        return new ASCIIEncoding().GetString(array);
    }

    private static void ValidateInput(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Invalid hash Id", "id");
        }
    }
}
