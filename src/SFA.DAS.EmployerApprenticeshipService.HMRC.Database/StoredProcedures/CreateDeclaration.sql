CREATE PROCEDURE [levy].[CreateDeclaration]
	@LevyDueYtd DECIMAL (18,4), 
	@EmpRef NVARCHAR(20), 
	@SubmissionDate DATETIME, 
	@SubmissionId BIGINT, 
	@AccountId BIGINT,
	@LevyAllowanceForYear DECIMAL(18, 4),
	@PayrollYear NVARCHAR(10),
	@PayrollMonth TINYINT
AS
	

INSERT INTO [levy].[LevyDeclaration] 
	(
		LevyDueYtd, 
		empRef, 
		SubmissionDate, 
		SubmissionId, 
		AccountId,
		LevyAllowanceForYear,
		PayrollYear,
		PayrollMonth
	) 
VALUES 
	(
		@LevyDueYtd, 
		@EmpRef, 
		@SubmissionDate, 
		@SubmissionId, 
		@AccountId,
		@LevyAllowanceForYear,
		@PayrollYear,
		@PayrollMonth
	);