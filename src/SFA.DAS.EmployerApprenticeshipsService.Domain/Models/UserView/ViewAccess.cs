using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.UserView
{
    public class ViewAccess
    {
        public string ViewName { get; set; }

        public List<string> EmailAddresses { get; set; }
        
    }
}