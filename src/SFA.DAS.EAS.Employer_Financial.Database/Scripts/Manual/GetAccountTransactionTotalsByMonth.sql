DECLARE @AccountId BIGINT = 1

SELECT 
  DATEPART(year, DateCreated) AS Year, DATEPART(month, DateCreated) AS Month,
  SUM(CASE WHEN TransactionType = 1 THEN Amount ELSE 0 END) AS 'Levy' ,
  SUM(CASE WHEN TransactionType = 2 THEN Amount ELSE 0 END) AS 'TopUp' ,
  SUM(CASE WHEN TransactionType = 3 THEN Amount ELSE 0 END) AS 'Payment' ,
  SUM(CASE WHEN TransactionType = 4 THEN Amount ELSE 0 END) AS 'Transfer'
FROM employer_financial.TransactionLine
WHERE AccountId = @AccountId
AND DateCreated < '2018-11-01 0:00:00'
GROUP BY DATEPART(year, DateCreated), DATEPART(month, DateCreated)
ORDER BY 1,2