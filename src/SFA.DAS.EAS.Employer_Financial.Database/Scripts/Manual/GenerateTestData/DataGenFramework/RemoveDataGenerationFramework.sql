
-- Stored Procedures

IF (OBJECT_ID('DataGen.CreatePeriodEnd', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreatePeriodEnd
GO

IF (OBJECT_ID('DataGen.CreatePayment', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreatePayment
GO

IF (OBJECT_ID('DataGen.CreateAccountPayments', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreateAccountPayments
GO

IF (OBJECT_ID('DataGen.CreateTransfer', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreateTransfer
GO

IF (OBJECT_ID('DataGen.CreateAccountTransferTransaction', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreateAccountTransferTransaction
GO

IF (OBJECT_ID('DataGen.ProcessPaymentDataTransactionsGenerateDataEdition', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.ProcessPaymentDataTransactionsGenerateDataEdition
GO

IF (OBJECT_ID('DataGen.CreatePaymentsForMonth', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreatePaymentsForMonth
GO

IF (OBJECT_ID('DataGen.CreatePaymentAndTransferForMonth', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreatePaymentAndTransferForMonth
GO

IF (OBJECT_ID('DataGen.CreatePaymentsForMonths', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreatePaymentsForMonths
GO

IF (OBJECT_ID('DataGen.CreateTransfersForMonths', 'P') IS NOT NULL)
  DROP PROCEDURE DataGen.CreateTransfersForMonths
GO

-- Functions

IF object_id(N'DataGen.CalendarPeriodMonth', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CalendarPeriodMonth
GO

IF object_id(N'DataGen.CalendarPeriodYear', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CalendarPeriodYear
GO

IF object_id(N'DataGen.PayrollMonth', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PayrollMonth
GO

IF object_id(N'DataGen.PayrollYear', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PayrollYear
GO

IF object_id(N'DataGen.CollectionPeriodMonth', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CollectionPeriodMonth
GO

IF object_id(N'DataGen.CollectionPeriodYear', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CollectionPeriodYear
GO

IF object_id(N'DataGen.PeriodEndMonth', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PeriodEndMonth
GO

IF object_id(N'DataGen.PeriodEndYear', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PeriodEndYear
GO

IF object_id(N'DataGen.PeriodEnd', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PeriodEnd
GO

IF object_id(N'DataGen.GenerateSourceTable', N'TF') IS NOT NULL
    DROP FUNCTION DataGen.GenerateSourceTable
GO

IF object_id(N'DataGen.GenerateLevySourceTable', N'TF') IS NOT NULL
    DROP FUNCTION DataGen.GenerateLevySourceTable
GO

-- Types

drop type if exists DataGen.PaymentGenerationSourceTable
go

drop type if exists DataGen.LevyGenerationSourceTable
go

-- Schema

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'DataGen')
BEGIN
	EXEC('DROP SCHEMA DataGen')
END
GO
