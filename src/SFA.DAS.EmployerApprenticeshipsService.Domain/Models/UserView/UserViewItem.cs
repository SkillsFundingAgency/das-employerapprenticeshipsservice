using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.UserView
{
    public class UserViewItem
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool SplitAccessAcrossUsers { get; set; }
        public List<ViewAccess> Views { get; set; }
    }
}