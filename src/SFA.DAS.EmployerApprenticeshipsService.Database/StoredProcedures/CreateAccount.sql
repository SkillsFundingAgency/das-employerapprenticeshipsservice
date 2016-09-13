CREATE PROCEDURE [account].[CreateAccount]
(
	@userId BIGINT,
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRef NVARCHAR(16),
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@accountId BIGINT OUTPUT,
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50),
	@AddedDate DATETIME
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @legalEntityId BIGINT;
	DECLARE @employerAgreementId BIGINT;

	INSERT INTO [account].[Account](Name) VALUES (@employerName);
	SELECT @accountId = SCOPE_IDENTITY();

	SELECT @legalEntityId = Id FROM [account].[LegalEntity] WHERE Code = @employerNumber;
	
	IF (@legalEntityId IS NULL)
	BEGIN
		INSERT INTO [account].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation) VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation);
		SELECT @legalEntityId = SCOPE_IDENTITY();	
	END
	
	DECLARE @templateId INT;

	SELECT TOP 1 @templateId = Id FROM [account].[EmployerAgreementTemplate] ORDER BY Id ASC;

	INSERT INTO [account].[EmployerAgreement](LegalEntityId, TemplateId, StatusId) VALUES (@legalEntityId, @templateId, 1);
	SELECT @employerAgreementId = SCOPE_IDENTITY();

	INSERT INTO [account].[AccountEmployerAgreement](AccountId, EmployerAgreementId) VALUES (@accountId, @employerAgreementId);

	INSERT INTO [account].[Paye](Ref, LegalEntityId, AccessToken, RefreshToken) VALUES (@employerRef, @legalEntityId, @accessToken, @refreshToken);

	EXEC [account].[CreateAccountHistory] @accountId, @employerRef,@AddedDate

	INSERT INTO [account].[Membership](UserId, AccountId, RoleId) VALUES (@userId, @accountId, 1);
END