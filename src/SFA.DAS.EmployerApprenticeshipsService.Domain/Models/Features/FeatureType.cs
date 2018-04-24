namespace SFA.DAS.EAS.Domain.Models.Features
{
    public enum FeatureType
    {
        NotSpecified = 0,
        Transfers = 1,
        Activities = 2,
        Projections = 3,
        Recruitments = 4,

        // These enums are only used in unit tests - the numbers can be changed 
        Test1 = 100,
        Test2 = 101
    }
}