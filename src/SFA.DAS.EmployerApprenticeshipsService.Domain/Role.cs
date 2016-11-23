using System;

namespace SFA.DAS.EAS.Domain
{
    public enum Role : short
    {
        None = 0,
        Owner = 1,
        Transactor = 2,
        Viewer = 3
    }

    public static class RoleStrings {
        public static string GetRoleDescription(Role role) { return GetRoleDescription(role.ToString()); }
        public static string GetRoleDescription(short roleId) { return GetRoleDescription((Role)roleId); }
        public static string GetRoleDescription(string role)
        {
            switch(role)
            {
                case "Owner": return "Invite team members, sign agreements, add apprentices and view information";
                case "Transactor": return "Add apprentices and view information";
                case "Viewer": return "View information but can’t make changes";
                default: throw new ArgumentException("Unexpected role: " + role);
            }
        }
        
        public static string GetRoleDescriptionToLower(short roleId) { return GetRoleDescriptionToLower((Role)roleId); }
        public static string GetRoleDescriptionToLower(Role role) { return GetRoleDescriptionToLower(role.ToString()); }
        public static string GetRoleDescriptionToLower(string role)
        {
            var str = GetRoleDescription(role);
            return char.ToLower(str[0]) + str.Substring(1);
        }


    }
}