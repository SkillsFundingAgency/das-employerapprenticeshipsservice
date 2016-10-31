using System;

namespace SFA.DAS.EAS.Domain
{
    public enum Role : short
    {
        Owner = 1,
        Transactor = 2,
        Viewer = 3
    }

    public static class RoleStrings {
        public static string ToWhatTheyCanDo(Role role) { return ToWhatTheyCanDo(role.ToString()); }
        public static string ToWhatTheyCanDo(short roleId) { return ToWhatTheyCanDo((Role)roleId); }
        public static string ToWhatTheyCanDo(string role)
        {
            switch(role)
            {
                case "Owner": return "Everything - they're the owner";
                case "Transactor": return "Create and stop payments and view financial information";
                case "Viewer": return "View financial information";
                default: throw new ArgumentException("Unexpected role: " + role);
            }
        }
        public static string ToWhatYouCanDo(string role)
        {
            switch (role)
            {
                case "Owner": return "Everything - you're the owner";
                case "Transactor": return "Create and stop payments and view financial information";
                case "Viewer": return "View financial information";
                default: throw new ArgumentException("Unexpected role: " + role);
            }
        }


        public static string ToWhatTheyCanDoLower(short roleId) { return ToWhatTheyCanDoLower((Role)roleId); }
        public static string ToWhatTheyCanDoLower(Role role) { return ToWhatTheyCanDoLower(role.ToString()); }
        public static string ToWhatTheyCanDoLower(string role)
        {
            var str = ToWhatTheyCanDo(role);
            return char.ToLower(str[0]) + str.Substring(1);
        }


    }
}