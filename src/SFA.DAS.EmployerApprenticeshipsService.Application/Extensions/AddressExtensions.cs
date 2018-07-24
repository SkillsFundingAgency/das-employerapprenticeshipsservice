using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.ReferenceData.Types.DTO;

namespace SFA.DAS.EAS.Application.Extensions
{
    public static class AddressExtensions
    {
        public static string FormatAddress(this Address address)
        {
            var sb = new StringBuilder();

            void AddIfNotNull(string s)
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

            AddIfNotNull(address.Line1);
            AddIfNotNull(address.Line2);
            AddIfNotNull(address.Line3);
            AddIfNotNull(address.Line4);
            AddIfNotNull(address.Line5);
            AddIfNotNull(address.Postcode);

            return sb.ToString();
        }
    }
}
