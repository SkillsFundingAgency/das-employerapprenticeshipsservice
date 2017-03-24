CREATE PROCEDURE [employer_account].[CreateLegalEntity]
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@status varchar(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@sector NVARCHAR(100) NULL,
	@legalEntityId BIGINT OUTPUT
AS
BEGIN
	INSERT INTO [employer_account].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation, [Status], [Source], [PublicSectorDataSource], Sector)
	VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation,@status, @source, @publicSectorDataSource,@sector);	
	SELECT @legalEntityId = SCOPE_IDENTITY();
END