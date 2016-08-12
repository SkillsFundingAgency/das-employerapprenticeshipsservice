CREATE PROCEDURE [dbo].[CreateLegalEntity]
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@legalEntityId BIGINT OUTPUT
AS
BEGIN
	INSERT INTO [dbo].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation) VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation);
	SELECT @legalEntityId = SCOPE_IDENTITY();
END