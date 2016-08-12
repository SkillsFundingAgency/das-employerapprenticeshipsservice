CREATE PROCEDURE [dbo].[SignEmployerAgreement]
	@agreementId BIGINT,
	@signedById BIGINT,
	@signedByName NVARCHAR(100)
AS
	SET NOCOUNT ON;

	UPDATE [dbo].[EmployerAgreement] 
	SET SignedById = @signedById, 
		SignedByName = @signedByName, 
		SignedDate = GETDATE(), 
		StatusId = 2 
	WHERE Id = @agreementId