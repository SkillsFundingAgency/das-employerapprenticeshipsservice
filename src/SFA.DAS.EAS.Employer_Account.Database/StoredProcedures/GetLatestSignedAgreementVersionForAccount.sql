CREATE PROCEDURE [employer_account].[GetLatestSignedAgreementVersionForAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	MAX(VersionNumber) 
	  FROM	employer_account.EmployerAgreement as EA
			JOIN employer_account.EmployerAgreementTemplate AS EAT 
				ON EAT.Id = EA.TemplateId 
	 WHERE	AccountId = @accountId AND 
			StatusId = 2

END

