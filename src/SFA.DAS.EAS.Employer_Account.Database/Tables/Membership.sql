CREATE TABLE [employer_account].[Membership]
(
    [AccountId] BIGINT NOT NULL, 
    [UserId] BIGINT NOT NULL, 
    [RoleId] TINYINT NOT NULL, 
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_Membership_Account] FOREIGN KEY (AccountId) REFERENCES [employer_account].[Account]([Id]), 
    CONSTRAINT [FK_Membership_User] FOREIGN KEY (UserId) REFERENCES [employer_account].[User]([Id]), 
    CONSTRAINT [FK_Membership_Role] FOREIGN KEY (RoleId) REFERENCES [employer_account].[Role]([Id]), 
    CONSTRAINT [PK_Membership] PRIMARY KEY ([UserId], [AccountId])
)
