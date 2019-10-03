namespace SFA.DAS.EmployerAccounts.Api.UnitTests
{
    public static class ObjectReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object o, string propertyName)
        {
            return
                (T) o.GetType().GetProperty(propertyName).GetValue(
                    o,
                    null);
        }
    }
}