CREATE VIEW [levy].[GetLevyDeclarations]
AS

SELECT 
	ld.Id AS Id,	
	ld.AccountId as AccountId,
	ld.empRef AS EmpRef,
	ld.SubmissionDate AS SubmissionDate,
	ld.SubmissionId AS SubmissionId,
	ld.LevyDueYTD AS LevyDueYTD,
	t.Amount AS EnglishFraction
FROM [levy].[LevyDeclaration] ld
OUTER APPLY
(
	SELECT TOP 1 Amount
	FROM [levy].[EnglishFraction] ef
	WHERE ef.EmpRef = ld.empRef 
		AND ef.[DateCalculated] <= ld.[SubmissionDate]
	ORDER BY [DateCalculated] DESC
) t



