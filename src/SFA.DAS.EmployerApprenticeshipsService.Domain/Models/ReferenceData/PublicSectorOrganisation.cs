namespace SFA.DAS.EAS.Domain.Models.ReferenceData
{
    public class PublicSectorOrganisation
    {
        public string Name { get; set; }
        public DataSource Source { get; set; }

        public string Sector { get; set; }
    }

    public enum DataSource : short
    {
        Ons = 1,
        Nhs = 2,
        Police = 3
    }
}
