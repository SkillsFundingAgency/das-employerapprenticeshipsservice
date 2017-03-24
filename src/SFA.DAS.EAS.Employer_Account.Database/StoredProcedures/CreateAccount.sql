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

	if(@employerNumber is not null)
	BEGIN
		SELECT @legalEntityId = Id FROM [employer_account].[LegalEntity] WHERE Code = @employerNumber and Source in (1,2);
	END

	IF (@legalEntityId IS NULL)
	BEGIN
		INSERT INTO [employer_account].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation, [Status], [Source], [PublicSectorDataSource],[Sector]) VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation,@status, @source, @publicSectorDataSource,@sector);
		SELECT @legalEntityId = SCOPE_IDENTITY();	
	END
	
	EXEC [employer_account].[CreateEmployerAgreement] @legalEntityId, @accountId, @employerAgreementId OUTPUT
	
	INSERT INTO [employer_account].[AccountEmployerAgreement](AccountId, EmployerAgreementId) VALUES (@accountId, @employerAgreementId);

	IF EXISTS(select 1 from [employer_account].[Paye] where ref = @employerRef)
	BEGIN
		EXEC [employer_account].[UpdatePaye] @employerRef,@accessToken, @refreshToken,@employerRefName
	END
	ELSE
	BEGIN
		EXEC [employer_account].[CreatePaye] @employerRef,@accessToken, @refreshToken,@employerRefName
	END

	EXEC [employer_account].[CreateAccountHistory] @accountId, @employerRef,@addedDate

	INSERT INTO [employer_account].[Membership](UserId, AccountId, RoleId) VALUES (@userId, @accountId, 1);
END