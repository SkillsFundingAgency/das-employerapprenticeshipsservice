--
-- GET TRANSACTIONS THAT ARE INCORRECT
--

--WITH AllPays (AccountId, Ukprn, PeriodEnd, Amount)
--AS
--(
--    SELECT AccountId, Ukprn, PeriodEnd, SUM(p.Amount) as Amount
--    FROM [employer_financial]‌‌‌.[Payment] p
--    WHERE FundingSource in (1)        
--    GROUP By AccountId, Ukprn, PeriodEnd
--)

--Select p.AccountId, p.PeriodEnd, p.Ukprn, p.Amount, tl.Amount, p.Amount + tl.Amount as Delta from AllPays p
--INNER JOIN [employer_financial]‌‌‌.[TransactionLine] tl
--    ON p.AccountId = tl.AccountId
--        AND p.Ukprn = tl.UkPrn
--        AND p.PeriodEnd = tl.PeriodEnd
--        AND tl.TransactionType = 3
--Where  ABS(p.Amount + tl.Amount) > 0.001
--ORDER BY AccountId, Ukprn, PeriodEnd

---------------------------------------------------------------------


--
-- SEE WHICH TRANSACTIONS ARE GETTING MODIFIED
--

--SELECT 
--tl.AccountId,
--tl.PeriodEnd,
--tl.UkPrn,
--tl.Amount,
--result.Amount AS NewAmount	,
--(tl.Amount - result.Amount) as DIFF 
--FROM employer_financial.TransactionLine tl
--INNER JOIN 
--(	
--	SELECT  
--		p.AccountId,
--		p.PeriodEnd,
--		p.Ukprn,
--		SUM(ISNULL(Amount, 0)) * -1 AS Amount		
--	FROM 
--		employer_financial.[Payment] p		
--		WHERE p.FundingSource = 1				
--	GROUP BY					
--		p.Ukprn, p.PeriodEnd, p.AccountId		
--) AS result ON tl.AccountId = result.AccountId AND tl.PeriodEnd = result.PeriodEnd AND tl.UkPrn = result.Ukprn
--WHERE tl.PeriodEnd = '1718-R07'
--AND tl.TransactionType = 3
--AND ABS(tl.Amount - result.Amount) > 0.0001
--ORDER BY tl.AccountId, tl.Ukprn, tl.PeriodEnd

---------------------------------------------------------------------


--
-- UPDATE TRANSACTIONS TO CORRECT VALUES
--

UPDATE employer_financial.TransactionLine
SET
    Amount = result.Amount	
FROM employer_financial.TransactionLine tl
INNER JOIN 
(	
    SELECT  
        p.AccountId,
        p.PeriodEnd,
        p.Ukprn,
        SUM(ISNULL(Amount, 0)) * -1 AS Amount		
    FROM 
        employer_financial.[Payment] p		
        WHERE p.FundingSource = 1				
    GROUP BY					
        p.Ukprn, p.PeriodEnd, p.AccountId		
) AS result ON tl.AccountId = result.AccountId AND tl.PeriodEnd = result.PeriodEnd AND tl.UkPrn = result.Ukprn
WHERE tl.PeriodEnd = '1718-R07'
AND tl.TransactionType = 3
AND ABS(tl.Amount - result.Amount) > 0.0001