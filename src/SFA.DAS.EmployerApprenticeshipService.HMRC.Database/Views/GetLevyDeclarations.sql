CREATE VIEW [levy].[GetLevyDeclarations]
AS
SELECT ld.Id AS Id,	
	ld.empRef AS EmpRef,
	ld.SubmissionDate AS SubmissionDate,
	ld.SubmissionId AS SubmissionId,
	ld.SubmissionType AS SubmissionType,
	ld.Amount AS Amount,
	t.Amount AS EnglishFraction
FROM [levy].[LevyDeclaration] ld
OUTER APPLY
(
	SELECT TOP 1 *
	FROM [levy].[EnglishFraction] ef
	WHERE ef.EmpRef = ld.empRef 
		AND ef.[DateCalculated] <= ld.[SubmissionDate]
	ORDER BY [DateCalculated] DESC
) t