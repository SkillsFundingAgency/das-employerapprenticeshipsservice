CREATE PROCEDURE [employer_account].[GetLatestSignedAgreementVersionForAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MIN(VersionNumber) AS LatestAgreement
	FROM	( 
			SELECT	ea.LegalEntityId, ea.AccountId, MAX(EAT.VersionNumber) AS VersionNumber
			FROM	employer_account.EmployerAgreement as EA
					JOIN employer_account.EmployerAgreementTemplate AS EAT 
						ON EAT.Id = EA.TemplateId 
			WHERE	EA.AccountId = @accountId
					and EA.StatusId = 2
			GROUP BY EA.LegalEntityId, EA.AccountId) AS T1
END

