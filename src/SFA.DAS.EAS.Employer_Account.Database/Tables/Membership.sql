CREATE TABLE [employer_account].[Membership]
(
    [AccountId] BIGINT NOT NULL, 
    [UserId] BIGINT NOT NULL, 
    [RoleId] INT NOT NULL, 
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ShowWizard] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Membership_Account] FOREIGN KEY (AccountId) REFERENCES [employer_account].[Account]([Id]), 
    CONSTRAINT [FK_Membership_User] FOREIGN KEY (UserId) REFERENCES [employer_account].[User]([Id]), 
    CONSTRAINT [PK_Membership] PRIMARY KEY ([UserId], [AccountId])
)

GO

CREATE NONCLUSTERED INDEX [IX_Membership_AccountIdRoleId] ON [employer_account].[Membership] ([AccountId], [RoleId]) INCLUDE ([CreatedDate]) WITH (ONLINE = ON)
