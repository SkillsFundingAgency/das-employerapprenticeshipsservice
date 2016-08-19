CREATE PROCEDURE [account].[GetLegalEntitiesLinkedToAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT le.*
	FROM [account].[LegalEntity] le
		JOIN [account].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [account].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
	WHERE aea.AccountId = @accountId;
END

