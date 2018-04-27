CREATE PROCEDURE [employer_account].[GetStatistics]

AS
SELECT 
(
	SELECT COUNT(Id) 
	FROM [employer_account].[Account]
) AS TotalAccounts,
(
	SELECT COUNT(Id)
	FROM [employer_account].[LegalEntity]
	WHERE Status = 'active'
) AS TotalLegalEntities,
(
	SELECT COUNT(Ref)
	FROM [employer_account].[Paye]
) AS TotalPAYESchemes,
(
	SELECT COUNT(Id) 
	FROM [employer_account].[EmployerAgreement]
	WHERE StatusId = 2
) AS TotalAgreements