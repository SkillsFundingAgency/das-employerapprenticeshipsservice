CREATE PROCEDURE [employer_account].[CreateLegalEntityWithAgreement]
	@accountId BIGINT,
	@companyNumber NVARCHAR(50),
	@companyName NVARCHAR(255),
	@companyAddress NVARCHAR(255),
	@companyDateOfIncorporation DATETIME,
	@status VARCHAR(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@legalEntityId BIGINT OUTPUT,
	@employerAgreementId BIGINT OUTPUT,
	@sector NVARCHAR(100) NULL,
	@accountLegalentityId BIGINT OUTPUT,
	@accountLegalEntityCreated BIT OUTPUT
AS
BEGIN	
	DECLARE @firstName NVARCHAR(MAX)	
	DECLARE @lastName NVARCHAR(MAX)
	DECLARE @signedByName NVARCHAR(100)

	SELECT @legalEntityId = Id FROM [employer_account].[LegalEntity] WHERE Code = @companyNumber AND Source = @source

	IF(@legalEntityId IS NULL)
	BEGIN
		EXEC [employer_account].[CreateLegalEntity] 
			@code=@companyNumber,
			@dateOfIncorporation=@companyDateOfIncorporation,
			@status=@status,
			@source=@source,
			@publicSectorDataSource=@publicSectorDataSource,
			@sector=@sector,
			@legalEntityId=@legalEntityId OUTPUT	
	END

	EXEC [employer_account].[CreateAccountLegalEntity]
			@accountId = @accountId,
			@legalEntityId = @legalEntityId,
			@employerName = @companyName,
			@employerRegisteredAddress = @companyAddress,
			@accountLegalEntityId = @accountLegalEntityId OUTPUT,
			@accountLegalEntityCreated = @accountLegalEntityCreated OUTPUT

	EXEC [employer_account].[CreateEmployerAgreement] 
			@accountLegalEntityId = @accountLegalEntityId, 
			@templateId = NULL, 
			@employerAgreementId = @employerAgreementId OUTPUT
END