using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class LegalEntityDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string RegisteredAddress { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public string Status { get; set; }
        public byte Source { get; set; }
        public byte? PublicSectorDataSource { get; set; }
        public string Sector { get; set; }
    }
}
