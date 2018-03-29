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
) AS TotalActiveLegalEntities,
(
	SELECT COUNT(Ref)
	FROM [employer_account].[Paye]
) AS TotalPayeSchemes,
(
	SELECT COUNT(Id) 
	FROM [employer_account].[EmployerAgreement]
	WHERE StatusId = 2
) AS TotalSignedAgreements,
(
	SELECT COUNT(PaymentId) 
	FROM [employer_financial].[Payment]
	WHERE CollectionPeriodYear = YEAR(GETDATE())
) AS TotalPaymentsThisYear