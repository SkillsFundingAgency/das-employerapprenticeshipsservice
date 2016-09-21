CREATE PROCEDURE [account].[CreateLegalEntityWithAgreement]
	@accountId BIGINT,
	@companyNumber NVARCHAR(50),
	@companyName NVARCHAR(255),
	@companyAddress NVARCHAR(255),
	@companyDateOfIncorporation DATETIME,
	@signAgreement BIT,
	@signedDate DATETIME,
	@signedById BIGINT,
	@legalEntityId BIGINT OUTPUT,
	@employerAgreementId BIGINT OUTPUT
AS
BEGIN	
	DECLARE @firstName NVARCHAR(MAX)	
	DECLARE @lastName NVARCHAR(MAX)
	DECLARE @signedByName NVARCHAR(100)	

	EXEC [account].[CreateLegalEntity] @companyNumber,@companyName,@companyAddress,@companyDateOfIncorporation,@legalEntityId OUTPUT	

	EXEC [account].[CreateEmployerAgreement] @legalEntityId, @employerAgreementId OUTPUT

	EXEC [account].[CreateAccountEmployerAgreement] @accountId, @employerAgreementId	

	IF (@signAgreement = 1) 	
	BEGIN			
		EXEC [account].[SignEmployerAgreement] @employerAgreementId, @signedById, @signedDate
	END	
END