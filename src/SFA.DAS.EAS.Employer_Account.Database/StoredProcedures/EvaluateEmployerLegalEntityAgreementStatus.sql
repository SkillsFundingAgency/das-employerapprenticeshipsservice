CREATE PROCEDURE [employer_account].[EvaluateEmployerLegalEntityAgreementStatus]
	@accountId bigint,
	@legalEntityId bigint
AS
    /*
    *  The inner most SQL finds the highest versioned agreement for the account/legal entity for pending and signed status (values 0 and 1).
    *  The next SQL out then aggregates these (at most two rows) into a single row with separate fields for pending and signed agreement details
    *  The outer SQL (the update) uses the two pending and two signed fields to update the relevant account / legal entity row.
    */
    UPDATE	T1
    SET		T1.PendingAgreementId = CASE WHEN T2.IsPendingEffective=1 THEN T2.PendingAgreementId ELSE NULL END,
		    T1.PendingAgreementVersion = CASE WHEN T2.IsPendingEffective=1 THEN T2.PendingAgreementVersion ELSE NULL END,
		    T1.SignedAgreementId = T2.SignedAgreementId,
		    T1.SignedAgreementVersion = T2.SignedAgreementVersion 
    FROM	employer_account.AccountLegalEntity AS T1
		    LEFT JOIN (
				SELECT	T4.AccountLegalEntityId,
						T4.PendingAgreementId, T4.PendingAgreementVersion, 
						T4.SignedAgreementId, T4.SignedAgreementVersion,
						CASE
							WHEN IsNull(T4.PendingAgreementVersion, -2) < IsNull(T4.SignedAgreementVersion, -1) THEN 0 
							ELSE 1
						END AS IsPendingEffective
				FROM  (
					SELECT	AccountLegalEntityId,
							MAX(CASE WHEN StatusId = 1 THEN AgreementId ELSE NULL END) AS PendingAgreementId,
							MAX(CASE WHEN StatusId = 1 THEN VersionNumber ELSE NULL END) AS PendingAgreementVersion,
							MAX(CASE WHEN StatusId = 2 THEN AgreementId ELSE NULL END) AS SignedAgreementId,
							MAX(CASE WHEN StatusId = 2 THEN VersionNumber ELSE NULL END) AS SignedAgreementVersion
					FROM	(
								-- This subqeury will return at most two rows - up to 1 pending row and up to 1 signed row. If there are more than one then the highest version wins
								SELECT	EA.StatusId, ROW_NUMBER() OVER(PARTITION BY ALE.Id, StatusId order by VersionNumber desc) AS RowNumber, VersionNumber, EA.Id as AgreementId, ALE.Id as AccountLegalEntityId
								FROM	employer_account.AccountLegalEntity AS ALE
										JOIN employer_account.EmployerAgreement AS EA
											ON EA.AccountLegalEntityId = ALE.Id
										JOIN employer_account.EmployerAgreementTemplate AS EAT
											ON EAT.Id = EA.TemplateId
								WHERE	EA.StatusId IN (1, 2)
							) AS T3
					WHERE T3.RowNumber = 1
					GROUP BY AccountLegalEntityId
					) AS T4
			) AS T2
				ON T2.AccountLegalEntityId = T1.ID
    WHERE	t1.AccountId = @accountId
		    AND t1.LegalEntityId = @legalEntityId;


GO;

