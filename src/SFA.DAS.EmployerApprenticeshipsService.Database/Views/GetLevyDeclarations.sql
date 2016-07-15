CREATE VIEW [dbo].[GetLevyDeclarations]
AS
SELECT ld.Id AS Id,
	a.Id AS AccountId,
	ld.empRef AS EmpRef,
	ld.SubmissionDate AS SubmissionDate,
	ld.SubmissionId AS SubmissionId,
	ld.SubmissionType AS SubmissionType,
	ld.Amount AS Amount,
	t.Amount AS EnglishFraction
FROM [dbo].[LevyDeclaration] ld
	JOIN [dbo].[Paye] p 
		ON p.Ref = ld.empRef
	JOIN [dbo].[Account] a
		ON a.Id = p.AccountId
	OUTER APPLY
	(
		SELECT TOP 1 *
		FROM [dbo].[EnglishFraction] ef
		WHERE ef.EmpRef = ld.empRef 
			AND ef.[DateCalculated] <= ld.[SubmissionDate]
		ORDER BY [DateCalculated] DESC
	) t