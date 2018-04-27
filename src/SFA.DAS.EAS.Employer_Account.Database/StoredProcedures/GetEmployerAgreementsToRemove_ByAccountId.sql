CREATE PROCEDURE [employer_account].[GetEmployerAgreementsToRemove_ByAccountId]
	@accountId BIGINT
AS
	SET NOCOUNT ON

	SELECT ISNULL(s.Id, p.Id) AS Id, le.Name, ISNULL(s.Status, p.Status) AS Status, a.HashedId, le.Code
	FROM [employer_account].[LegalEntity] le
	CROSS APPLY (
		SELECT a.HashedId
		FROM [employer_account].[Account] a
		INNER JOIN [employer_account].[EmployerAgreement] ea ON ea.AccountId = a.Id
		WHERE a.Id = @accountId
		AND ea.LegalEntityId = le.Id
		AND ea.StatusId IN (1, 2)
		GROUP BY a.HashedId
	) a
	OUTER APPLY (
		SELECT TOP 1 ea.Id, ea.StatusId AS Status
		FROM [employer_account].[EmployerAgreement] ea
		INNER JOIN [employer_account].[EmployerAgreementTemplate] eat ON eat.Id = ea.TemplateId
		WHERE ea.LegalEntityId = le.Id
		AND ea.StatusId = 2
		ORDER BY eat.VersionNumber DESC
	) s
	OUTER APPLY (
		SELECT TOP 1 ea.Id, ea.StatusId AS Status
		FROM [employer_account].[EmployerAgreement] ea
		INNER JOIN [employer_account].[EmployerAgreementTemplate] eat ON eat.Id = ea.TemplateId
		WHERE ea.LegalEntityId = le.Id
		AND ea.StatusId = 1
		ORDER BY eat.VersionNumber DESC
	) p