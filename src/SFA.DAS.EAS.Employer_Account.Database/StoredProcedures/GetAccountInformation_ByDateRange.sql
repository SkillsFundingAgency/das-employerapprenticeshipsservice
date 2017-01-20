CREATE PROCEDURE employer_account.GetAccountInformation_ByDateRange
	@fromDate DATETIME,
	@toDate DATETIME,
	@offset int,
	@pageSize int
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
WHERE AccountRemovedDate is null
and AccountCreatedDate BETWEEN @fromDate and @toDate
Order by AccountId
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY