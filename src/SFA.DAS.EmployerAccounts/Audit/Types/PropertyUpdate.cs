using System.Globalization;

namespace SFA.DAS.EmployerAccounts.Audit.Types;

public class PropertyUpdate
{
    public string PropertyName { get; set; }
    public string NewValue { get; set; }

    public static PropertyUpdate FromString(string propertyName, string newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue
        };
    }
    public static PropertyUpdate FromDateTime(string propertyName, DateTime newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString("yyyy-MM-dd HH:mm:ss.fffff")
        };
    }
    public static PropertyUpdate FromShort(string propertyName, short newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString()
        };
    }
    public static PropertyUpdate FromInt(string propertyName, int newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString()
        };
    }
    public static PropertyUpdate FromLong(string propertyName, long newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString()
        };
    }
    public static PropertyUpdate FromFloat(string propertyName, float newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString(CultureInfo.CurrentCulture)
        };
    }
    public static PropertyUpdate FromDouble(string propertyName, double newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString(CultureInfo.CurrentCulture)
        };
    }
    public static PropertyUpdate FromDecimal(string propertyName, decimal newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString(CultureInfo.CurrentCulture)
        };
    }
    public static PropertyUpdate FromBool(string propertyName, bool newValue)
    {
        return new PropertyUpdate
        {
            PropertyName = propertyName,
            NewValue = newValue.ToString().ToLower()
        };
    }
}
