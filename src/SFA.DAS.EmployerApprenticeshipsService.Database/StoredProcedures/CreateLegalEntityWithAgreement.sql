CREATE PROCEDURE [account].[CreateLegalEntityWithAgreement]
	@accountId BIGINT,
	@companyNumber NVARCHAR(50),
	@companyName NVARCHAR(255),
	@companyAddress NVARCHAR(255),
	@companyDateOfIncorporation DATETIME,
	@signAgreement BIT,
	@signedDate DATETIME,
	@signedById BIGINT,
	@status VARCHAR(50),
	@legalEntityId BIGINT OUTPUT,
	@employerAgreementId BIGINT OUTPUT
AS
BEGIN	
	DECLARE @firstName NVARCHAR(MAX)	
	DECLARE @lastName NVARCHAR(MAX)
	DECLARE @signedByName NVARCHAR(100)	

	EXEC [account].[CreateLegalEntity] @companyNumber,@companyName,@companyAddress,@companyDateOfIncorporation, @status,@legalEntityId OUTPUT	

	EXEC [account].[CreateEmployerAgreement] @legalEntityId, @employerAgreementId OUTPUT

	EXEC [account].[CreateAccountEmployerAgreement] @accountId, @employerAgreementId	

	IF (@signAgreement = 1) 	
	BEGIN	
		SELECT @firstName = FirstName, @lastName = LastName FROM [account].[User] WHERE Id = @signedById
		SELECT @signedByName =  @firstName + ' ' + @lastName
		
		EXEC [account].[SignEmployerAgreement] @employerAgreementId, @signedById, @signedByName, @signedDate
	END	
END