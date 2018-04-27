CREATE PROCEDURE [employer_account].[GetLegalEntity_ById]
	@accountId BIGINT,
	@legalEntityId BIGINT
AS
	SET NOCOUNT ON

	SELECT 
		le.Id, 
		le.Name, 
		le.RegisteredAddress AS Address, 
		le.[Status], 
		le.DateOfIncorporation AS DateOfInception, 
		le.Code,
		le.Source as SourceNumeric,
		CASE le.Source WHEN 1 THEN 'Companies House' WHEN 2 THEN 'Charities' WHEN 3 THEN 'Public Bodies' ELSE 'Other' END AS Source,
		CASE le.PublicSectorDataSource WHEN 1 THEN 'ONS' WHEN 2 THEN 'NHS' WHEN 3 THEN 'Police' ELSE '' END AS PublicSectorDataSource,
		le.Sector,
		lea.StatusId AS AgreementStatus,
		lea.SignedByName AS AgreementSignedByName,
		lea.SignedDate AS AgreementSignedDate
	FROM [employer_account].[LegalEntity] le
	OUTER APPLY
	(
		SELECT TOP 1 ea.*
		FROM [employer_account].[EmployerAgreement] ea
		INNER JOIN [employer_account].[EmployerAgreementTemplate] eat ON eat.Id = ea.TemplateId
		WHERE ea.LegalEntityId = le.Id
		AND ea.AccountId = @accountId
		ORDER BY eat.VersionNumber DESC, ea.Id DESC
	) lea
	WHERE le.Id = @legalEntityId