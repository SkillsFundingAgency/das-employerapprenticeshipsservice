namespace SFA.DAS.EAS.Domain.Models.Features
{
    public enum FeatureType
    {
        NotSpecified,
        Activities,
        Projections,
        Recruitments,
        TransferConnectionRequests,
        Transfers,

        // These enums are only used in unit tests - the numbers can be changed 
        Test1 = 100,
        Test2 = 101
    }
}