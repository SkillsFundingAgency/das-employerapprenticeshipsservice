CREATE TABLE [employer_financial].[AccountTransferAllowanceSnapshot]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AccountId] BIGINT NOT NULL, 
    [Year] SMALLINT NOT NULL, 
    [TransferAllowance] DECIMAL(18, 5) NOT NULL, 
    [SnapshotTime] DATETIME NOT NULL 
    )
GO

CREATE UNIQUE INDEX [IX_AccountTransferAllowanceSnapshot_AccountYear]
ON [employer_financial].[AccountTransferAllowanceSnapshot]([AccountId] ASC, [Year] ASC)
GO

