CREATE PROCEDURE [levy].[ProcessDeclarationsTransactions]
AS

-- Create Declarations

INSERT INTO levy.TransactionLine
	
select 
		x.AccountId,
		x.SubmissionId,
		null,
		x.SubmissionDate,
		1,
		(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.EnglishFraction as Amount
	FROM 
		[levy].[GetLevyDeclarations] x
	inner join (
		select submissionId from levy.LevyDeclaration
	EXCEPT
		select SubmissionId from levy.TransactionLine where TransactionType = 1
	) dervx on dervx.submissionId = x.SubmissionId
	left join 
		[levy].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
		y.LevyDueYTD is not null
	
	

--create top up
	
INSERT INTO levy.TransactionLine
	
select 
		x.AccountId,
		x.SubmissionId,
		null,
		x.SubmissionDate,
		2,
		(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.TopUpPercentage as Amount
		
	FROM 
		[levy].[GetLevyDeclarations] x
	inner join (
		select submissionId from levy.LevyDeclaration
	EXCEPT
		select SubmissionId from levy.TransactionLine where TransactionType = 2
	) dervx on dervx.submissionId = x.SubmissionId
	left join 
		[levy].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
		y.LevyDueYTD is not null
	
