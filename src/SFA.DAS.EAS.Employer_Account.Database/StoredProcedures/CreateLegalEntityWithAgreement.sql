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
	@sector NVARCHAR(100) NULL
AS
BEGIN	
	DECLARE @firstName NVARCHAR(MAX)	
	DECLARE @lastName NVARCHAR(MAX)
	DECLARE @signedByName NVARCHAR(100)	

	SELECT @legalEntityId = Id FROM [employer_account].[LegalEntity] WHERE Code = @companyNumber and Source = @source

	IF(@legalEntityId is null)
	BEGIN
		EXEC [employer_account].[CreateLegalEntity] 
		@companyNumber,@companyName,@companyAddress,
		@companyDateOfIncorporation, @status, @source, @publicSectorDataSource,@sector, @legalEntityId OUTPUT	
	END

	EXEC [employer_account].[CreateEmployerAgreement] @legalEntityId, @accountId, @employerAgreementId OUTPUT

	EXEC [employer_account].[CreateAccountEmployerAgreement] @accountId, @employerAgreementId	

	INSERT INTO [employer_account].[UserLegalEntitySettings] (UserId, EmployerAgreementId, ReceiveNotifications)
	select m.UserId, a.Id, 1
	from [employer_account].[EmployerAgreement] a
	join [employer_account].[Membership] m on m.AccountId = a.AccountId
	where a.Id = @employerAgreementId

END