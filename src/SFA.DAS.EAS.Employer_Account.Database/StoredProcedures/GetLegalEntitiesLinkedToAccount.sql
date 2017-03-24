CREATE PROCEDURE [employer_account].[GetLegalEntitiesLinkedToAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT le.*
	FROM [employer_account].[LegalEntity] le
		JOIN [employer_account].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [employer_account].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
	WHERE aea.AccountId = @accountId;
END

