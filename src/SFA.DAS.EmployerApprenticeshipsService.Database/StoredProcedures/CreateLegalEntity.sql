CREATE PROCEDURE [account].[CreateLegalEntity]
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRegisteredAddress NVARCHAR(256),
	@employerDateOfIncorporation DATETIME,
	@companyStatus varchar(50),
	@legalEntityId BIGINT OUTPUT
AS
BEGIN
	INSERT INTO [account].[LegalEntity](Name, Code, RegisteredAddress, DateOfIncorporation, CompanyStatus) 
	VALUES (@employerName, @employerNumber, @employerRegisteredAddress, @employerDateOfIncorporation,@companyStatus);	
	SELECT @legalEntityId = SCOPE_IDENTITY();
END