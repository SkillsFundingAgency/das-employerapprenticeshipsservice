CREATE PROCEDURE [employer_account].[GetEmployerAgreementsToRemove_ByAccountId]
	@accountId BIGINT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT	ISNULL(Signed.Id, Pending.Id) AS Id,
			ALE.Name, 
			ISNULL(Signed.StatusId, Pending.StatusId) AS Status, 
			a.HashedId, 
			le.Code
	FROM	employer_account.AccountLegalEntity AS ALE
			JOIN employer_account.Account AS A
				ON A.Id = ALE.AccountId
			JOIN employer_account.LegalEntity AS LE
				ON LE.Id = ALE.LegalEntityId
			LEFT JOIN employer_account.EmployerAgreement AS Pending
				ON Pending.AccountLegalEntityId = ALE.ID 
					AND Pending.StatusId = 1 
			LEFT JOIN employer_account.EmployerAgreement AS Signed
				ON Signed.AccountLegalEntityId = ALE.ID
					AND Signed.StatusId = 2
	WHERE	ALE.AccountId = 6; 

END;