CREATE TABLE [dbo].[AccountEmployerAgreement]
(
	[AccountId] BIGINT NOT NULL, 
    [EmployerAgreementId] BIGINT NOT NULL, 
    CONSTRAINT [PK_AccountEmployerAgreement] PRIMARY KEY ([EmployerAgreementId], [AccountId]), 
    CONSTRAINT [FK_AccountEmployerAgreement_Account] FOREIGN KEY ([AccountId]) REFERENCES [Account]([Id]), 
    CONSTRAINT [FK_AccountEmployerAgreement_EmployerAgreement] FOREIGN KEY ([EmployerAgreementId]) REFERENCES [EmployerAgreement]([Id]) 
)
