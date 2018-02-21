CREATE PROCEDURE [employer_account].[GetAccount_ByPublicHashedId]
	@publicHashedAccountId VARCHAR(100)	
AS
	SELECT [Id], [HashedId], [Name], [CreatedDate], [ModifiedDate], [PublicHashedId]
	FROM [employer_account].[Account]
	WHERE [PublicHashedId] = @publicHashedAccountId;