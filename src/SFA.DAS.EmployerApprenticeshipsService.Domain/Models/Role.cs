namespace SFA.DAS.EAS.Domain.Models;

public enum Role
{
    None = 0,
    Owner = 1,
    Transactor = 2,
    Viewer = 3
}

public static class RoleStrings
{
    public const string NoneDescription = "Minimal access for Employer services";
    public const string OwnerDescription = "Accept agreements, view information and manage PAYE schemes, organisations, apprentices and team members";
    public const string TransactorDescription = "Add apprentices and view information";
    public const string ViewerDescription = "View information but can’t make changes";
    
    public static string GetRoleDescription(Role role)
    {
        return GetRoleDescription(role.ToString());
    }

    public static string GetRoleDescription(string role)
        => role switch
        {
            "None" => NoneDescription,
            "Owner" => OwnerDescription,
            "Transactor" => TransactorDescription,
            "Viewer" => ViewerDescription,
            _ => throw new ArgumentException("Unexpected role: " + role)
        };

    public static string GetRoleDescriptionToLower(Role role)
    {
        return GetRoleDescriptionToLower(role.ToString());
    }

    public static string GetRoleDescriptionToLower(string role)
    {
        var str = GetRoleDescription(role);
        return char.ToLower(str[0]) + str.Substring(1);
    }
}
