CREATE PROCEDURE [dbo].[GetLegalEntitiesLinkedToAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT le.*
	FROM [dbo].[LegalEntity] le
		JOIN [dbo].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [dbo].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
	WHERE aea.AccountId = @accountId;
END

