DECLARE @accounts TABLE(AccountId BIGINT, PayeRef NVARCHAR(20))

INSERT INTO @accounts (AccountId, PayeRef) VALUES
(16824, '120/GA09231'),
(23525, '585/NY20698'),
(23483, '951/O6024')

UPDATE t
SET
	t.LevyDeclared = u.LevyDeclared,
	t.Amount = u.LevyDeclared * t.EnglishFraction * 1.1
FROM [employer_financial].[TransactionLine] t
INNER JOIN (
	SELECT
		t2.AccountId,
		t2.EmpRef,
		t2.SubmissionId,
		((
			SELECT TOP 1 LevyDueYTD
			FROM [employer_financial].[LevyDeclaration]
			WHERE AccountId = t2.AccountId
			AND EmpRef = t2.EmpRef
			ORDER BY CreatedDate ASC
		) - (
			SELECT TOP 1 LevyDueYTD
			FROM [employer_financial].[LevyDeclaration]
			WHERE AccountId = x.AccountId
			AND EmpRef = t2.EmpRef
			ORDER BY CreatedDate DESC
		)) AS LevyDeclared
	FROM [employer_financial].[TransactionLine] t2
	INNER JOIN @accounts a ON a.AccountId = t2.AccountId AND a.PayeRef = t2.EmpRef
	CROSS APPLY (
		SELECT TOP 1 t3.AccountId, t3.LevyDeclared
		FROM [employer_financial].[TransactionLine] t3
		WHERE t3.EmpRef = t2.EmpRef
		AND t3.AccountId <> t2.AccountId
		ORDER BY t3.DateCreated DESC
	) x
	WHERE t2.TransactionType = 1
	AND t2.DateCreated >= '2018-03-15 00:00:00.000'
	AND t2.DateCreated < '2018-03-24 00:00:00.000'
) u ON u.AccountId = t.AccountId AND u.EmpRef = t.EmpRef AND u.SubmissionId = t.SubmissionId