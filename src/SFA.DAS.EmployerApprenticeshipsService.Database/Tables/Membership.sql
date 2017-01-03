CREATE TABLE [account].[Membership]
(
    [AccountId] BIGINT NOT NULL, 
    [UserId] BIGINT NOT NULL, 
    [RoleId] TINYINT NOT NULL, 
	[CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_Membership_Account] FOREIGN KEY (AccountId) REFERENCES [account].[Account]([Id]), 
    CONSTRAINT [FK_Membership_User] FOREIGN KEY (UserId) REFERENCES [account].[User]([Id]), 
    CONSTRAINT [FK_Membership_Role] FOREIGN KEY (RoleId) REFERENCES [account].[Role]([Id]), 
    CONSTRAINT [PK_Membership] PRIMARY KEY ([UserId], [AccountId])
)
