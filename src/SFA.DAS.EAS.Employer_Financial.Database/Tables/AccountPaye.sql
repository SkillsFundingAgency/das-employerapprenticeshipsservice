CREATE TABLE [employer_financial].[AccountPaye]
(
	[AccountId] BIGINT NOT NULL,
	[EmpRef] NVARCHAR(16), 
    [Aorn] VARCHAR(25) NULL,
	CONSTRAINT PK_AccountEmpRef PRIMARY KEY (AccountId, EmpRef)
)

