CREATE PROCEDURE [employer_account].[CreateLegalEntityWithAgreement]
	@accountId BIGINT,
	@companyNumber NVARCHAR(50),
	@companyName NVARCHAR(255),
	@companyAddress NVARCHAR(255),
	@companyDateOfIncorporation DATETIME,
	@signAgreement BIT,
	@signedDate DATETIME,
	@signedById BIGINT,
	@status VARCHAR(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@legalEntityId BIGINT OUTPUT,
	@employerAgreementId BIGINT OUTPUT,
	@sector NVARCHAR(100) NULL
AS
BEGIN	
	DECLARE @firstName NVARCHAR(MAX)	
	DECLARE @lastName NVARCHAR(MAX)
	DECLARE @signedByName NVARCHAR(100)	

	EXEC [employer_account].[CreateLegalEntity] 
	@companyNumber,@companyName,@companyAddress,
	@companyDateOfIncorporation, @status, @source, @publicSectorDataSource,@sector, @legalEntityId OUTPUT	

	EXEC [employer_account].[CreateEmployerAgreement] @legalEntityId, @employerAgreementId OUTPUT

	EXEC [employer_account].[CreateAccountEmployerAgreement] @accountId, @employerAgreementId	

	IF (@signAgreement = 1) 	
	BEGIN	
		SELECT @firstName = FirstName, @lastName = LastName FROM [employer_account].[User] WHERE Id = @signedById
		SELECT @signedByName =  @firstName + ' ' + @lastName
		
		EXEC [employer_account].[SignEmployerAgreement] @employerAgreementId, @signedById, @signedByName, @signedDate
	END	
END