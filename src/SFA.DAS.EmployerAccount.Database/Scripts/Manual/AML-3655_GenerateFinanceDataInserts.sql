SELECT 'INSERT INTO employer_financial.account (Id, Name)' +
	   ' VALUES (' + CAST(Id AS VARCHAR) + ', ''' + REPLACE(Name, '''', '''''') + ''')'
FROM employer_account.Account

SELECT 'INSERT INTO employer_financial.AccountPaye (AccountId, EmpRef, Name, Aorn)' +
	   ' VALUES (' + CAST(AccountId AS VARCHAR) + ', ''' + PayeRef + ''', ''' + REPLACE(ISNULL(p.Name, ''), '''', '''''') + ''',' + CASE WHEN Aorn IS NULL THEN 'NULL' ELSE '''' + Aorn + '''' END + ')'
FROM employer_account.AccountHistory ah
INNER JOIN employer_account.Paye p ON p.Ref = ah.PayeRef
WHERE ah.RemovedDate IS NULL

SELECT 'INSERT INTO employer_financial.AccountLegalEntity (Id, AccountId, LegalEntityId, SignedAgreementVersion, SignedAgreementId, PendingAgreementId)' +
	   ' VALUES (' + CAST(Id AS VARCHAR) + ', ' +
					 CAST(AccountId AS VARCHAR) + ',' + 
					 CAST(LegalEntityId AS VARCHAR) + ',' + 
					 CASE WHEN SignedAgreementVersion IS NULL THEN 'NULL' ELSE CAST(SignedAgreementVersion AS VARCHAR) END + ',' + 
					 CASE WHEN SignedAgreementId IS NULL THEN 'NULL' ELSE CAST(SignedAgreementId AS VARCHAR) END + ',' + 
					 CASE WHEN PendingAgreementVersion IS NULL THEN 'NULL' ELSE CAST(PendingAgreementVersion AS VARCHAR) END + ')'
FROM employer_account.AccountLegalEntity
WHERE Deleted IS NULL