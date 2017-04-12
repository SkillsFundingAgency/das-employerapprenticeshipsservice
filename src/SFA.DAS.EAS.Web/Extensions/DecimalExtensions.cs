namespace SFA.DAS.EAS.Web.Extensions
{
    public static class DecimalExtensions
    {
        public static string FormatCost(this decimal? value)
        {
            return !value.HasValue ? string.Empty : $"£{value.Value:n0}";
        }
    }
}