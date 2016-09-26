namespace System
{
    public static class DateTimeExtensions
    {
        public static string ToGdsFormat(this DateTime date)
        {
            return date.ToString("d MMM yyyy");
        }
    }
}