namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public enum AuthorizationResult
    {
        Ok,
        FeatureDisabled,
        FeatureAgreementNotSigned,
        FeatureUserNotWhitelisted
    }
}