CREATE PROCEDURE [employer_financial].[CreateDeclaration]
	@LevyDueYtd DECIMAL (18,4), 
	@EmpRef NVARCHAR(20), 
	@SubmissionDate DATETIME, 
	@SubmissionId BIGINT, 
	@HmrcSubmissionId BIGINT,
	@AccountId BIGINT,
	@LevyAllowanceForYear DECIMAL(18, 4),
	@PayrollYear NVARCHAR(10),
	@PayrollMonth TINYINT,
	@CreatedDate DATETIME,
	@DateCeased DATETIME = NULL,
	@InactiveFrom DATETIME = NULL,
	@InactiveTo DATETIME = NULL,
	@EndOfYearAdjustment BIT,
	@EndOfYearAdjustmentAmount DECIMAL(18,4),
	@NoPaymentForPeriod BIT
AS
	

INSERT INTO [employer_financial].[LevyDeclaration] 
	(
		LevyDueYTD, 
		EmpRef, 
		SubmissionDate, 
		SubmissionId, 
		AccountId,
		LevyAllowanceForYear,
		PayrollYear,
		PayrollMonth,
		CreatedDate,
		EndOfYearAdjustment,
		EndOfYearAdjustmentAmount,
		DateCeased,
		InactiveFrom,
		InactiveTo,
		HmrcSubmissionId,
		NoPaymentForPeriod
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
		@CreatedDate,
		@EndOfYearAdjustment,
		@EndOfYearAdjustmentAmount,
		@DateCeased,
		@InactiveFrom,
		@InactiveTo,
		@HmrcSubmissionId,
		@NoPaymentForPeriod
	);