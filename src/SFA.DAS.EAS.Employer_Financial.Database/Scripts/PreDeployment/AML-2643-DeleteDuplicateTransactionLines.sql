IF (NOT EXISTS (
	SELECT 1
	FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'employer_financial'
	AND TABLE_NAME = 'AML-2643-TransactionLine-NonUnique')
)
BEGIN
	SELECT t.*
	INTO [employer_financial].[AML-2643-TransactionLine-NonUnique] 
	FROM [employer_financial].[TransactionLine] t
	INNER JOIN (
		SELECT SubmissionId
		FROM [employer_financial].[TransactionLine]
		WHERE TransactionType = 1
		GROUP BY SubmissionId
		HAVING COUNT(1) > 1
	) dt
	ON dt.SubmissionId = t.SubmissionId

	DELETE FROM dt
	FROM (
		SELECT ROW_NUMBER() OVER (PARTITION BY SubmissionId ORDER BY (SELECT 0)) RowNumber
		FROM [employer_financial].[TransactionLine]
		WHERE TransactionType = 1
	) dt
	WHERE dt.RowNumber > 1
END