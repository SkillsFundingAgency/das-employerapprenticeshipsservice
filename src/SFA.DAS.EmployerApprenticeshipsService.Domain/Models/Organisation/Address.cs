using System.Text;

namespace SFA.DAS.EAS.Domain.Models.Organisation
{
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Line5 { get; set; }
        public string Postcode { get; set; }

        public string GetAddress()
        {

            var addressBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Line1))
            {
                addressBuilder.Append($"{Line1}, ");
            }

            if (!string.IsNullOrEmpty(Line2))
            {
                addressBuilder.Append($"{Line2}, ");
            }

            if (!string.IsNullOrEmpty(Line3))
            {
                addressBuilder.Append($"{Line3}, ");
            }

            if (!string.IsNullOrEmpty(Line4))
            {
                addressBuilder.Append(string.IsNullOrEmpty(Postcode)
                    ? $"{Line4}"
                    : $"{Line4}, ");
            }

            if (!string.IsNullOrEmpty(Postcode))
            {
                addressBuilder.Append(Postcode);
            }

            return addressBuilder.ToString();

        }
    }
}
