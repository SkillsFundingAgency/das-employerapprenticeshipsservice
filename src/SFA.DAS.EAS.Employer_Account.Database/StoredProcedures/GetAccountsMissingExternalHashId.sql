CREATE PROCEDURE [employer_account].[GetAccountsMissingExternalHashId]
AS

SELECT Id FROM [employer_account].[Account] WHERE ExternalHashedId IS NULL
