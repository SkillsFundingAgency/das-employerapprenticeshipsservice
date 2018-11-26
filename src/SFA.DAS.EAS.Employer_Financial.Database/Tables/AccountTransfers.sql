CREATE TABLE [employer_financial].[AccountTransfers]
(
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [SenderAccountId] BIGINT NOT NULL, 
    [SenderAccountName] NVARCHAR(100) NOT NULL, 
    [ReceiverAccountId] BIGINT NOT NULL, 
    [ReceiverAccountName] NVARCHAR(100) NOT NULL, 
    [ApprenticeshipId] BIGINT NOT NULL, 	
    [CourseName] VARCHAR(MAX) NOT NULL, 
    [CourseLevel] INT,
    [PeriodEnd] nvarchar(20) NOT NULL,
    [Amount] DECIMAL(18, 5) NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL,   
    [CreatedDate] DATETIME NOT NULL, 
    [RequiredPaymentId] UNIQUEIDENTIFIER NOT NULL,   
    )
GO

CREATE INDEX [IX_AccountTransferSenderAccountId]
ON [employer_financial].[AccountTransfers]([SenderAccountId] ASC)
GO

CREATE INDEX [IX_AccountTransferReceiverTransfers]
ON [employer_financial].[AccountTransfers]([ReceiverAccountId] ASC, [PeriodEnd] ASC)
GO

CREATE UNIQUE INDEX [IX_PeriodEndAccountTransfer]
ON [employer_financial].[AccountTransfers]([ApprenticeshipId] ASC, [PeriodEnd] ASC)
GO

