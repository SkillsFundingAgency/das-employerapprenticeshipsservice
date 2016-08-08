using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class EmployerAccountPayeListViewModel
    {
        public long AccountId { get; set; }
                    
        public List<PayeView> PayeSchemes { get; set; }   
    }
}