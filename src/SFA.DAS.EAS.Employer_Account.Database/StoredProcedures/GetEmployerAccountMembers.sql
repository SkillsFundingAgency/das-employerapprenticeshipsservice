CREATE PROCEDURE [employer_account].[GetEmployerAccountMembers]
	@hashedAccountId VARCHAR(MAX)	
AS
	select tm.* from [employer_account].[GetTeamMembers] tm 
    where tm.HashedId = @hashedAccountId
GO