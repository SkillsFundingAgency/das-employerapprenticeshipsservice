CREATE PROCEDURE [employer_account].[GetAccountsMissingPublicHashedId]
AS
	SELECT Id
	FROM [employer_account].[Account]
	WHERE PublicHashedId IS NULL