using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class EmployerAccountPayeListViewModel
    {
        public string HashedId { get; set; }
                    
        public List<PayeView> PayeSchemes { get; set; }
    }
}