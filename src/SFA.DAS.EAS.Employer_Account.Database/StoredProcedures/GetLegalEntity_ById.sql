CREATE PROCEDURE employer_account.GetLegalEntity_ById
	@Id BIGINT
AS
Select 
	le.Id, 
	le.Name, 
    le.RegisteredAddress AS [Address], 
	le.[Status], 
	le.DateOfIncorporation AS DateOfInception, 
	le.Code,
    CASE le.Source WHEN 1 THEN 'Companies House' WHEN 2 THEN 'Charities' WHEN 3 THEN 'Public Bodies' ELSE 'Other' END AS Source,
	CASE le.PublicSectorDataSource WHEN 1 THEN 'ONS' WHEN 2 THEN 'NHS' WHEN 3 THEN 'Police' ELSE '' END as PublicSectorDataSource,
	le.Sector,
	lea.StatusId AS AgreementStatus,
	lea.SignedByName AS AgreementSignedByName,
	lea.SignedDate AS AgreementSignedDate
from employer_account.LegalEntity le
OUTER APPLY
(
	SELECT TOP 1 *
	FROM employer_account.EmployerAgreement ea
	WHERE ea.LegalEntityId = le.Id
	ORDER BY Id DESC
) lea
WHERE le.Id = @Id