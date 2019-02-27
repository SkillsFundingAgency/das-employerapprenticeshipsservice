﻿CREATE TYPE [employer_financial].[PaymentsTable] AS TABLE
(
	PaymentId UNIQUEIDENTIFIER NOT NULL,
	Ukprn BIGINT NOT NULL,
	ProviderName VARCHAR(MAX) NULL,
	Uln BIGINT NOT NULL,
	AccountId BIGINT NOT NULL,
	ApprenticeshipId BIGINT NOT NULL,
	DeliveryPeriodMonth INT NOT NULL,
	DeliveryPeriodYear INT NOT NULL,
	CollectionPeriodId CHAR(20) NOT NULL,
	CollectionPeriodMonth INT NOT NULL,
	CollectionPeriodYear INT NOT NULL,
	EvidenceSubmittedOn DATETIME NOT NULL,
	EmployerAccountVersion VARCHAR(50) NOT NULL,
	ApprenticeshipVersion VARCHAR(25) NOT NULL,
	FundingSource VARCHAR(25) NOT NULL,
	TransactionType VARCHAR(25) NOT NULL,
	Amount DECIMAL(18,5) NOT NULL,
	PeriodEnd VARCHAR(25) NOT NULL,
	StandardCode BIGINT NULL,
	FrameworkCode INT NULL,
	ProgrammeType INT NULL,
	PathwayCode INT NULL,
	PathwayName VARCHAR(MAX) NULL,
	ApprenticeshipCourseName VARCHAR(MAX) NULL,
	ApprenticeName VARCHAR(MAX) NULL,
	ApprenticeNINumber VARCHAR(MAX) NULL,
	ApprenticeshipCourseLevel INT NULL,
	ApprenticeshipCourseStartDate DATETIME NULL,
	PaymentDetails VARCHAR(MAX) NULL,
	PRIMARY KEY (PaymentId ASC)
)