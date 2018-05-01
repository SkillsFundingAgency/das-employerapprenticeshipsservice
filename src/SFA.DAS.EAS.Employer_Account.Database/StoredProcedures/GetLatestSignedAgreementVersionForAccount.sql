CREATE PROCEDURE [employer_account].[GetLatestSignedAgreementVersionForAccount]
	@accountId BIGINT
AS
	SET NOCOUNT ON

	SELECT
		CASE
			WHEN MIN(v.VersionNumber) = 0 THEN NULL
			ELSE MIN(v.VersionNumber)
		END AS LatestSignedAgreementVersionNumber
	FROM (
		SELECT
			CASE
				WHEN a.StatusId = 1 THEN 0
				WHEN a.StatusId = 2 THEN MAX(t.VersionNumber)
			END AS VersionNumber
		FROM employer_account.EmployerAgreement as a
		JOIN employer_account.EmployerAgreementTemplate AS t ON t.Id = a.TemplateId 
		WHERE a.AccountId = @accountId
		AND a.StatusId IN (1, 2)
		GROUP BY a.AccountId, a.LegalEntityId, a.StatusId
	) AS v