CREATE PROCEDURE [account].[CreateLegalEntity]
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@status varchar(50),
	@source TINYINT,
	@publicSectorDataSource TINYINT,
	@legalEntityId BIGINT OUTPUT
AS
BEGIN
	INSERT INTO [account].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation, [Status], [Source], [PublicSectorDataSource])
	VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation,@status, @source, @publicSectorDataSource);	
	SELECT @legalEntityId = SCOPE_IDENTITY();
END