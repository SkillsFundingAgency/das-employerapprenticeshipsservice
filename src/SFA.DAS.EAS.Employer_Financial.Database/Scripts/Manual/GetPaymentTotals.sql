DECLARE @periodEnd NVARCHAR(20) = ''

SELECT
	SUM(CASE WHEN FundingSource = 1 THEN Amount ELSE 0 END) AS 'Levy payments total' ,
    SUM(CASE WHEN FundingSource = 2 THEN Amount ELSE 0 END) AS 'SFA co-funded payments total' ,
    SUM(CASE WHEN FundingSource = 3 THEN Amount ELSE 0 END) AS 'Employer co-funded payments total',
    SUM(CASE WHEN FundingSource = 5 THEN Amount ELSE 0 END) AS 'Levy transfer payments total',
    SUM(CASE WHEN FundingSource IN (1,5) THEN Amount ELSE 0 END) AS 'Expected levy transaction total'
FROM [employer_financial].[Payment]
WHERE PeriodEnd = @periodEnd

SELECT
	SUM(Amount) AS 'Levy transactions total',
    SUM(SfaCoInvestmentAmount) AS 'SFA co-funded transactions total',
    SUM(EmployerCoInvestmentAmount) AS 'Employer co-funded transactions total'
FROM [employer_financial].[TransactionLine]
WHERE TransactionType = 3
AND PeriodEnd = @periodEnd