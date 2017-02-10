CREATE PROCEDURE [employer_financial].[CreateDeclaration]
	@LevyDueYtd DECIMAL (18,4), 
	@EmpRef NVARCHAR(20), 
	@SubmissionDate DATETIME, 
	@SubmissionId BIGINT, 
	@AccountId BIGINT,
	@LevyAllowanceForYear DECIMAL(18, 4),
	@PayrollYear NVARCHAR(10),
	@PayrollMonth TINYINT,
	@CreatedDate DATETIME
AS
	

INSERT INTO [employer_financial].[LevyDeclaration] 
	(
		LevyDueYtd, 
		empRef, 
		SubmissionDate, 
		SubmissionId, 
		AccountId,
		LevyAllowanceForYear,
		PayrollYear,
		PayrollMonth,
		CreatedDate
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
		@PayrollMonth,
		@CreatedDate
	);