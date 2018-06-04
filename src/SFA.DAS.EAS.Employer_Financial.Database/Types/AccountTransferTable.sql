CREATE TYPE [employer_financial].[AccountTransferTable] AS TABLE
(
	SenderAccountId BIGINT NOT NULL,
	SenderAccountName NVARCHAR(100) NOT NULL,
	ReceiverAccountId BIGINT NOT NULL,
	ReceiverAccountName NVARCHAR(100) NOT NULL,
	ApprenticeshipId BIGINT NOT NULL,
	CourseName VARCHAR(MAX) NOT NULL,
	CourseLevel INT,
	Amount DECIMAL(18,5) NOT NULL,
	PeriodEnd NVARCHAR(20) NOT NULL,
	Type NVARCHAR(50) NOT NULL,	
	RequiredPaymentId UNIQUEIDENTIFIER NOT NULL
)