CREATE PROCEDURE [employer_account].[GetEmployerAgreementsToRemove_ByAccountId]
	@accountId BIGINT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT	ISNULL(Signed.Id, Pending.Id) AS Id,
			le.Name, 
			ISNULL(Signed.StatusId, Pending.StatusId) AS Status, 
			a.HashedId, 
			le.Code
	FROM	employer_account.AccountLegalEntity AS ALE
			JOIN employer_account.Account AS A
				on A.Id = ALE.AccountId
			JOIN employer_account.LegalEntity AS LE
				on LE.Id = ALE.LegalEntityId
			LEFT JOIN (SELECT TOP 1 Id, StatusId, AccountLegalEntityId FROM employer_account.EmployerAgreement WHERE StatusId = 1) AS Pending
				on Pending.AccountLegalEntityId = ALE.Id
			LEFT JOIN (SELECT TOP 1 Id, StatusId, AccountLegalEntityId FROM employer_account.EmployerAgreement WHERE StatusId = 2) AS Signed
				on Signed.AccountLegalEntityId = ALE.Id
	WHERE	ALE.AccountId = @accountId; 

END;