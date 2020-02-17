CREATE PROCEDURE [employer_account].[GetEmployerAgreementsToRemove_ByAccountId]
	@accountId BIGINT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT	ISNULL(ale.SignedAgreementId, ale.PendingAgreementId) AS Id,
			ALE.Name, 
			CASE WHEN ale.SignedAgreementId IS NOT NULL THEN 2 ELSE 1 END AS Status, 
			a.HashedId, 
			le.Code AS LegalEntityCode,
			le.[Source] AS LegalEntitySource
	FROM	employer_account.AccountLegalEntity AS ALE
			INNER JOIN employer_account.Account AS A ON A.Id = ALE.AccountId
			INNER JOIN employer_account.LegalEntity AS LE ON LE.Id = ALE.LegalEntityId
	WHERE	ALE.Deleted IS NULL AND ALE.AccountId = @accountId; 

END;