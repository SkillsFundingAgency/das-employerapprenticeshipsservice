using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Web.Models
{
    public class EmployerAccountPayeListViewModel
    {
        public string HashedId { get; set; }
                    
        public List<PayeView> PayeSchemes { get; set; }   
    }
}