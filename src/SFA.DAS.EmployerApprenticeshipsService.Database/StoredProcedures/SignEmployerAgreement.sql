CREATE PROCEDURE [account].[SignEmployerAgreement]
	@agreementId BIGINT,
	@signedById BIGINT,	
	@signedDate DateTime
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @legalEntityId BIGINT;

	UPDATE [account].[EmployerAgreement] 
	SET SignedById = @signedById, 		
		SignedDate = @signedDate, 
		StatusId = 2 
	WHERE Id = @agreementId;

	--mark previous agreement as superceded
	SELECT @legalEntityId = LegalEntityId
	FROM [account].[EmployerAgreement]
	WHERE Id = @agreementId;

	UPDATE [account].[EmployerAgreement] 
	SET StatusId = 4
	WHERE LegalEntityId = @legalEntityId
		AND Id <> @agreementId
		AND StatusId <> 4;
END