CREATE PROCEDURE [dbo].[SignEmployerAgreement]
	@agreementId BIGINT,
	@signedById BIGINT,
	@signedByName NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @legalEntityId BIGINT;

	UPDATE [dbo].[EmployerAgreement] 
	SET SignedById = @signedById, 
		SignedByName = @signedByName, 
		SignedDate = GETDATE(), 
		StatusId = 2 
	WHERE Id = @agreementId;

	--mark previous agreement as superceded
	SELECT @legalEntityId = LegalEntityId
	FROM [dbo].[EmployerAgreement]
	WHERE Id = @agreementId;

	UPDATE [dbo].[EmployerAgreement] 
	SET StatusId = 4
	WHERE LegalEntityId = @legalEntityId
		AND Id <> @agreementId
		AND StatusId <> 4;
END