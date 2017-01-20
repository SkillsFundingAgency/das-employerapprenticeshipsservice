CREATE PROCEDURE employer_account.GetAccountInformation_ByHashedId
	@HashedId VARCHAR(100)
AS
Select 
	DasAccountId, 
	DasAccountName, 
	DateRegistered, 
	OrganisationId, 
	OrganisationName, 
	OrganisationRegisteredAddress, 
	OrganisationStatus, 
	OrgansiationCreatedDate, 
	OrganisationNumber,
	OwnerEmail,
	OrganisationSource,
	PayeSchemeName
FROM [employer_account].[AccountInformation]
WHERE DasAccountId = @HashedId