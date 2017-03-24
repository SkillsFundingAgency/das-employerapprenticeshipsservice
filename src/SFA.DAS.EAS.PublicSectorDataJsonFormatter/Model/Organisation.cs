using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PublicSectorDataJsonFormatter.Model
{
    public class Organisation
    {
        public string Name { get; set; }
        public DataSource Source { get; set; }
        public string Sector { get; set; }
    }

    public enum DataSource
    {
        Ons = 1,
        Nhs = 2,
        Police = 3
    }

}
