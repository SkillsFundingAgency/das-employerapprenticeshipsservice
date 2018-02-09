CREATE PROCEDURE [employer_account].[GetAccount_ByExternalHashedId]
	@HashedAccountId VARCHAR(100)	
AS
SET NOCOUNT ON

SELECT [Id]
      ,[HashedId]
      ,[Name]
      ,[CreatedDate]
      ,[ModifiedDate]
      ,[ExternalHashedId]
FROM	[employer_account].[Account] a 
WHERE a.[ExternalHashedId] = @HashedAccountId;
