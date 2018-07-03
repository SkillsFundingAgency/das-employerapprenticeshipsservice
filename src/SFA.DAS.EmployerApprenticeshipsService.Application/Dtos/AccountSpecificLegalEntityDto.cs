using System;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class AccountSpecificLegalEntityDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public string Name { get; set; }
        public byte? PublicSectorDataSource { get; set; }
        public string RegisteredAddress { get; set; }
        public string Sector { get; set; }
        public byte Source { get; set; }
        public string Status { get; set; }
    }
}