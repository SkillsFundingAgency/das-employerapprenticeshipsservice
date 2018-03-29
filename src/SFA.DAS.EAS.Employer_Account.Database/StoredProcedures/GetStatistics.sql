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
	WHERE status = 'active'
) AS TotalLegalEntities,
(
	SELECT DISTINCT COUNT(Ref)
	FROM [employer_account].[Paye]
	GROUP BY Ref
) AS TotalPayeSchemes,
(
	SELECT COUNT(Id) 
	FROM [employer_account].[EmployerAgreement]
	WHERE StatusId = 2
) AS TotalAgreements,
(
	SELECT COUNT(PaymentId) 
	FROM [employer_financial].[Payment]
	WHERE CollectionPeriodYear = YEAR(GETDATE())
) AS TotalPayments