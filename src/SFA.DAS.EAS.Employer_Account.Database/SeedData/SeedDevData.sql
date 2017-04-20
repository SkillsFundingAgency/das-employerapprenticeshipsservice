/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

DECLARE @sqlStatement NVARCHAR(4000)
SELECT @sqlStatement = '
CREATE PROC #CreateAccount
	@accountId BIGINT,
	@userId BIGINT,
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRef NVARCHAR(16),
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@status varchar(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@employerRefName varchar(500) null,
	@sector NVARCHAR(100),
	@hashedAccountId NVARCHAR(50)
AS
BEGIN

	DECLARE @legalEntityId BIGINT, @employerAgreementId BIGINT

	IF (NOT EXISTS(SELECT * FROM [employer_account].[Account] WHERE Id = @accountId))
	BEGIN
		SET IDENTITY_INSERT  [employer_account].[Account] ON
		INSERT INTO [employer_account].[Account](Id, Name, CreatedDate) VALUES (@accountId, @employerName, GETDATE());
		UPDATE [employer_account].[Account] SET [HashedId] = @hashedAccountId WHERE [Id] = @accountId
		SET IDENTITY_INSERT  [employer_account].[Account] OFF

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
			EXEC [employer_account].[UpdatePaye] @employerRef,''accessToken'', ''refreshToken'',@employerRefName
		END
		ELSE
		BEGIN
			EXEC [employer_account].[CreatePaye] @employerRef,''accessToken'', ''refreshToken'',@employerRefName
		END

		DECLARE @tmp DATETIME
		SET @tmp = GETDATE()

		EXEC [employer_account].[CreateAccountHistory] @accountId, @employerRef, @tmp

		INSERT INTO [employer_account].[Membership](UserId, AccountId, RoleId) VALUES (@userId, @accountId, 1);
	END
END'

EXEC sp_executesql @sqlStatement

-- Account seed data
SET IDENTITY_INSERT  [employer_account].[Account] ON
IF (NOT EXISTS(SELECT * FROM [employer_account].[Account] WHERE Id = 1))
BEGIN 
    INSERT INTO [employer_account].[Account](Id, Name, HashedId, CreatedDate) 
    VALUES(1, 'ACME LTD', 'KAKAKAKA', GETDATE()) 
END 
ELSE 
BEGIN 
    UPDATE [employer_account].[Account] 
    SET Name = 'ACME LTD',
	HashedId = 'KAKAKAKA'
    WHERE Id = 1
END 
SET IDENTITY_INSERT  [employer_account].[Account] OFF

-- User seed data 
SET IDENTITY_INSERT  [employer_account].[User] ON
IF (NOT EXISTS(SELECT * FROM [employer_account].[User] WHERE Id = 1
	AND UserRef = '758943A5-86AA-4579-86AF-FB3D4A05850B'
    AND Email = 'floyd.price@test.local'))
BEGIN 
    INSERT INTO [employer_account].[User](Id, UserRef, Email, FirstName, LastName) 
    VALUES(1,'758943A5-86AA-4579-86AF-FB3D4A05850B','floyd.price@test.local', 'Floyd', 'Price') 
END 
ELSE 
BEGIN 
    UPDATE [employer_account].[User] 
    SET UserRef = '758943A5-86AA-4579-86AF-FB3D4A05850B', Email = 'floyd.price@test.local', FirstName = 'Floyd', LastName = 'Price'
    WHERE Id = 1
END 
SET IDENTITY_INSERT  [employer_account].[User] OFF

-- Membership seed data
IF (NOT EXISTS(SELECT * FROM [employer_account].[Membership] WHERE RoleId = 1
	AND [UserId] = 1
    AND AccountId = 1))
BEGIN 
    INSERT INTO [employer_account].[Membership](RoleId, UserId, AccountId) 
    VALUES(1,1,1) 
END

-- Template seed
SET IDENTITY_INSERT  [employer_account].[EmployerAgreementTemplate] ON
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V1'))
BEGIN 
	INSERT INTO [employer_account].[EmployerAgreementTemplate](Id, PartialViewName, CreatedDate) 
	VALUES(1, '_Agreement_V1', GETDATE()) 
END 
SET IDENTITY_INSERT  [employer_account].[EmployerAgreementTemplate] OFF

-- Users seeed data
DECLARE @userRef UNIQUEIDENTIFIER
SET @userRef = '87df36f4-78ad-47c7-84d7-900ef4c39920'
IF (NOT EXISTS(SELECT * FROM [employer_account].[User] WHERE UserRef = @userRef))
BEGIN
	EXECUTE [employer_account].[UpsertUser] @userRef, 'test@account.com', 'Test', 'Account' 
END

DECLARE @userId BIGINT
SELECT @userId = Id FROM [employer_account].[User] WHERE UserRef = @userRef

-- Account seed data
EXECUTE #CreateAccount 2, @userId, '00445790', 'Tesco Plc', '222/AA00002', 'Tesco House, Shire Park, Kestrel Way, Welwyn Garden City, AL7 1GA', '1947-11-27 00:00:00.000', 'active', 1, NULL, 'Employer for scenario 2 scheme 2', '', '84VBNV'
EXECUTE #CreateAccount 3, @userId, 'SC171417', 'SAINSBURY''S LIMITED', '123/SFAT029', 'No 2 Lochrin Square, 96 Fountainbridge, Edinburgh, EH3 9QA', '1997-01-16 00:00:00.000', 'active', 1, NULL, '', '', 'JLVKPM'
EXECUTE #CreateAccount 4, @userId, '07297044', 'DINE CONTRACT CATERING LIMITED', '101/CUR00016', '1st Floor The Centre, Birchwood Park, Warrington, Lancashire, WA3 6YN', '2010-06-28 00:00:00.000', 'active', 1, NULL, '', '', 'G6M7RV'

DROP PROCEDURE #CreateAccount