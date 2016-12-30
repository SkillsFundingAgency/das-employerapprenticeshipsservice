CREATE PROCEDURE [account].[CreateLegalEntity]
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@status varchar(50),
	@legalEntityId BIGINT OUTPUT
AS
BEGIN
	INSERT INTO [account].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation, [Status]) 
	VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation,@status);	
	SELECT @legalEntityId = SCOPE_IDENTITY();
END