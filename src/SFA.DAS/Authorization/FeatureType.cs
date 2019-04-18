namespace SFA.DAS.Authorization
{
    public enum FeatureType
    {
        NotSpecified,
        Activities,
        Projections,
        ProviderRelationships,
        Recruitments,
        TransferConnectionRequests,
        Transfers,

        /// <summary>
        /// Enables the New Registration journey
        /// </summary>
        NewRegistration,

        // These enums are only used in unit tests - the numbers can be changed 
        Test1 = 100,
        Test2 = 101
    }
}