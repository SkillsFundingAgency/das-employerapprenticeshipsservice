﻿CREATE PROCEDURE [account].[CreateAccount]
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
	@addedDate DATETIME	,
	@status varchar(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@employerRefName varchar(500) null
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @legalEntityId BIGINT;
	DECLARE @employerAgreementId BIGINT;

	INSERT INTO [account].[Account](Name, CreatedDate) VALUES (@employerName, @addedDate);
	SELECT @accountId = SCOPE_IDENTITY();

	SELECT @legalEntityId = Id FROM [account].[LegalEntity] WHERE Code = @employerNumber;
	
	IF (@legalEntityId IS NULL)
	BEGIN
		INSERT INTO [account].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation, [Status], [Source], [PublicSectorDataSource]) VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation,@status, @source, @publicSectorDataSource);
		SELECT @legalEntityId = SCOPE_IDENTITY();	
	END
	
	DECLARE @templateId INT;

	SELECT TOP 1 @templateId = Id FROM [account].[EmployerAgreementTemplate] ORDER BY Id ASC;

	INSERT INTO [account].[EmployerAgreement](LegalEntityId, TemplateId, StatusId) VALUES (@legalEntityId, @templateId, 2);
	SELECT @employerAgreementId = SCOPE_IDENTITY();
	
	INSERT INTO [account].[AccountEmployerAgreement](AccountId, EmployerAgreementId) VALUES (@accountId, @employerAgreementId);

	IF EXISTS(select 1 from [account].[Paye] where ref = @employerRef)
	BEGIN
		EXEC [account].[UpdatePaye] @employerRef,@accessToken, @refreshToken,@employerRefName
	END
	ELSE
	BEGIN
		EXEC [account].[CreatePaye] @employerRef,@accessToken, @refreshToken,@employerRefName
	END

	EXEC [account].[CreateAccountHistory] @accountId, @employerRef,@addedDate

	INSERT INTO [account].[Membership](UserId, AccountId, RoleId) VALUES (@userId, @accountId, 1);
END