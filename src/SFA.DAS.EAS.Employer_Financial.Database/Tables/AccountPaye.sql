CREATE TABLE [employer_financial].[AccountPaye]
(
	[AccountId] BIGINT NOT NULL,
	[EmpRef] NVARCHAR(16), 
	[Name] VARCHAR(500) NULL, 
    [Aorn] VARCHAR(25) NULL,
	CONSTRAINT PK_AccountEmpRef PRIMARY KEY (AccountId, EmpRef)
)
GO

CREATE INDEX [IX_AccountPaye_EmpRef] ON [employer_financial].[AccountPaye] ([EmpRef])
GO
CREATE INDEX [IX_AccountPaye_AccountId_Aorn] ON [employer_financial].[AccountPaye] ([AccountId], [Aorn])