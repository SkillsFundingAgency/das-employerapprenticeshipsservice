CREATE PROCEDURE [employer_account].[CreateAccount]
(
	@userId BIGINT,
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRef NVARCHAR(16),
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@accountId BIGINT OUTPUT,
	@legalEntityId BIGINT OUTPUT,
	@accountLegalentityId BIGINT OUTPUT,
	@employerAgreementId BIGINT OUTPUT,
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50),
	@addedDate DATETIME	,
	@status varchar(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@employerRefName varchar(500) null,
	@sector NVARCHAR(100)
)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [employer_account].[Account](Name, CreatedDate) VALUES (@employerName, @addedDate);
	SELECT @accountId = SCOPE_IDENTITY();

	DECLARE @accountLegalEntityCreated AS BIT;

	EXEC [employer_account].[CreateLegalEntityWithAgreement]
		@accountId = @accountId,
		@companyNumber = @employerNumber,
		@companyName = @employerName,
		@companyAddress = @employerRegisteredAddress,
		@companyDateOfIncorporation = @employerDateOfIncorporation,
		@status = @status,
		@source = @source,
		@publicSectorDataSource = @publicSectorDataSource,
		@sector = @sector,
		@legalEntityId = @legalEntityId OUTPUT,
		@employerAgreementId = @employerAgreementId OUTPUT,
		@accountLegalentityId = @accountLegalentityId OUTPUT,
		@accountLegalEntityCreated = @accountLegalEntityCreated OUTPUT;

	IF EXISTS(select 1 from [employer_account].[Paye] where Ref = @employerRef)
	BEGIN
		EXEC [employer_account].[UpdatePaye] @employerRef,@accessToken, @refreshToken,@employerRefName
	END
	ELSE
	BEGIN
		EXEC [employer_account].[CreatePaye] @employerRef,@accessToken, @refreshToken,@employerRefName
	END

	EXEC [employer_account].[CreateAccountHistory] @accountId, @employerRef,@addedDate

	INSERT INTO [employer_account].[Membership](UserId, AccountId, RoleId) VALUES (@userId, @accountId, 1);

	INSERT INTO [employer_account].[UserAccountSettings] (UserId, AccountId, ReceiveNotifications) VALUES (@userId, @accountId, 1)

END