CREATE PROCEDURE [levy].[ProcessDeclarationsTransactions]
AS

-- Create Declarations

INSERT INTO levy.TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId,
			GetDate() as TransactionDate,
			x.SubmissionId,
			null as PaymentId,
			x.SubmissionDate,
			1 as TransactionType,
			(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.EnglishFraction as Amount,
			x.EmpRef
		FROM 
			[levy].[GetLevyDeclarations] x
	
		left join 
			[levy].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
		where
			y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from levy.LevyDeclaration
	EXCEPT
		select SubmissionId from levy.TransactionLine where TransactionType = 1
	) dervx on dervx.submissionId = mainUpdate.SubmissionId

--create top up
	
INSERT INTO levy.TransactionLine
	select mainUpdate.* from
	(
select 
		x.AccountId,
		GetDate() as TransactionDate,
		x.SubmissionId,
		null as PaymentId,
		x.SubmissionDate,
		2 as TransactionType,
		((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.EnglishFraction) * y.TopUpPercentage as Amount,
		x.EmpRef
	FROM 
		[levy].[GetLevyDeclarations] x
	
	left join 
		[levy].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
		y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from levy.LevyDeclaration
	EXCEPT
		select SubmissionId from levy.TransactionLine where TransactionType = 2
	) dervx on dervx.submissionId = mainUpdate.SubmissionId
