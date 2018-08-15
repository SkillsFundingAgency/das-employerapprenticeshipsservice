

UPDATE	employer_account.AccountLegalEntity
SET		PublicHashedId = null
WHERE	PublicHashedId IS NOT NULL 
		AND Deleted IS NULL
