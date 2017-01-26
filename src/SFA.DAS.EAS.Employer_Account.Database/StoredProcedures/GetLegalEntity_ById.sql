CREATE PROCEDURE employer_account.GetLegalEntity_ById
	@Id BIGINT
AS
Select 
	le.Id, 
	le.Name, 
    le.RegisteredAddress, 
	le.[Status] AS CompanyStatus, 
	le.DateOfIncorporation, 
	le.Code,
    CASE le.Source WHEN 1 THEN 'Companies House' WHEN 2 THEN 'Charities' WHEN 3 THEN 'Public Bodies' ELSE 'Other' END AS Source
from employer_account.LegalEntity le
WHERE le.Id = @Id