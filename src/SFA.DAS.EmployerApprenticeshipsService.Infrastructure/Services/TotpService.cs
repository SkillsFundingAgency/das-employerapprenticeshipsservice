using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class TotpService : ITotpService
    {
        public string GetCode(string key, string timeValue = "")
        {
            var secret = Encoding.ASCII.GetBytes(key);
            var keyBytes = PadKeyToLength(secret, 64);

            var time = string.IsNullOrEmpty(timeValue)
                ? DateTime.UtcNow
                : DateTime.ParseExact(timeValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None);


            var timeSubtracted = ((long)Math.Floor((time - new DateTime(1970, 1, 1)).TotalSeconds / 30)).ToString("X");
            while (timeSubtracted.Length < 16)
            {
                timeSubtracted = "0" + timeSubtracted;
            }

            var timeBytes = StringToHexByteArray(timeSubtracted);

            var computedHash = new HMACSHA512(keyBytes).ComputeHash(timeBytes);

            var offset = computedHash[computedHash.Length - 1] & 0xf;

            var binary =
                ((computedHash[offset] & 0x7f) << 24) |
                ((computedHash[offset + 1] & 0xff) << 16) |
                ((computedHash[offset + 2] & 0xff) << 8) |
                (computedHash[offset + 3] & 0xff);

            var result = (binary % (int)Math.Pow(10, 8)).ToString();

            return result;
        }

        private byte[] StringToHexByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] hexAsBytes = new byte[hexString.Length / 2];
            for (var index = 0; index < hexAsBytes.Length; index++)
            {
                var byteValue = hexString.Substring(index * 2, 2);
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexAsBytes;
        }

        private byte[] PadKeyToLength(byte[] key, int requiredLength)
        {
            if (key.Length >= requiredLength)
            {
                return key;
            }

            var buffer = new byte[requiredLength];
            var offset = 0;
            while (offset < buffer.Length)
            {
                var copyLength = offset + key.Length > buffer.Length
                    ? buffer.Length - offset
                    : key.Length;
                Array.Copy(key, 0, buffer, offset, copyLength);
                offset += key.Length;
            }

            return buffer;
        }
    }
}
