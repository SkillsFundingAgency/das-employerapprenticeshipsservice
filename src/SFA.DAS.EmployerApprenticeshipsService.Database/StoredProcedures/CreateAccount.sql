CREATE PROCEDURE [dbo].[CreateAccount]
(
	@userId BIGINT,
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRef NVARCHAR(16),
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@accountId BIGINT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @legalEntityId BIGINT;
	DECLARE @employerAgreementId BIGINT;

	INSERT INTO [dbo].[Account](Name) VALUES (@employerName);
	SELECT @accountId = SCOPE_IDENTITY();

	SELECT @legalEntityId = Id FROM [dbo].[LegalEntity] WHERE Code = @employerNumber;
	
	IF (@legalEntityId IS NULL)
	BEGIN
		INSERT INTO [dbo].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation) VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation);
		SELECT @legalEntityId = SCOPE_IDENTITY();

		DECLARE @templateId INT;

		SELECT TOP 1 @templateId = Id FROM [dbo].[EmployerAgreementTemplate] ORDER BY Id ASC;

		INSERT INTO [dbo].[EmployerAgreement](LegalEntityId, TemplateId, StatusId) VALUES (@legalEntityId, @templateId, 1);
		SELECT @employerAgreementId = SCOPE_IDENTITY();
	END

	INSERT INTO [dbo].[AccountEmployerAgreement](AccountId, EmployerAgreementId) VALUES (@accountId, @employerAgreementId);

	INSERT INTO [dbo].[Paye](Ref, AccountId, LegalEntityId) VALUES (@employerRef, @accountId, @legalEntityId);
	INSERT INTO [dbo].[Membership](UserId, AccountId, RoleId) VALUES (@userId, @accountId, 1);
END