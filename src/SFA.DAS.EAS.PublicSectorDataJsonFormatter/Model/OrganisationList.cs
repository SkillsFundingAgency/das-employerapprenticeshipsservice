using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PublicSectorDataJsonFormatter.Model
{
    public class OrganisationList
    {
        public OrganisationList()
        {
            OrganisationNames = new List<Organisation>();
        }

        public List<Organisation> OrganisationNames { get; set; }
    }
}
