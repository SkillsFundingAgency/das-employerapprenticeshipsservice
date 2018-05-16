CREATE PROCEDURE [employer_account].[GetLatestSignedAgreementVersionForAccount]
	@accountId BIGINT
AS
	SET NOCOUNT ON

	SELECT
		CASE
			WHEN MIN(m.VersionNumber) = 0 THEN NULL
			ELSE MIN(m.VersionNumber)
		END AS LatestSignedAgreementVersionNumber
	FROM (
		SELECT MAX(v.VersionNumber) AS VersionNumber
		FROM (
			SELECT
				a.AccountId,
				a.LegalEntityId,
				CASE
					WHEN a.StatusId = 1 THEN 0
					WHEN a.StatusId = 2 THEN t.VersionNumber
				END AS VersionNumber
			FROM [employer_account].[EmployerAgreement] a
			JOIN [employer_account].[EmployerAgreementTemplate] t ON t.Id = a.TemplateId 
			WHERE a.AccountId = @accountId
			AND a.StatusId IN (1, 2)
		) v
		GROUP BY v.AccountId, v.LegalEntityId
	) m