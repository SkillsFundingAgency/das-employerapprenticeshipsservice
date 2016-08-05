CREATE TABLE [dbo].[Membership]
(
    [AccountId] BIGINT NOT NULL, 
    [UserId] BIGINT NOT NULL, 
    [RoleId] TINYINT NOT NULL, 
    CONSTRAINT [FK_Membership_Account] FOREIGN KEY (AccountId) REFERENCES [Account]([Id]), 
    CONSTRAINT [FK_Membership_User] FOREIGN KEY (UserId) REFERENCES [User]([Id]), 
    CONSTRAINT [FK_Membership_Role] FOREIGN KEY (RoleId) REFERENCES [Role]([Id]), 
    CONSTRAINT [PK_Membership] PRIMARY KEY ([UserId], [AccountId])
)
