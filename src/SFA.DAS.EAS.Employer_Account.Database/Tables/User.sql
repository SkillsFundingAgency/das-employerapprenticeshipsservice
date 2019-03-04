CREATE TABLE [employer_account].[User]
(
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [UserRef] UNIQUEIDENTIFIER NOT NULL, 
    [Email] NVARCHAR(255) NOT NULL UNIQUE, 
    [FirstName] NVARCHAR(MAX) NULL, 
    [LastName] NVARCHAR(MAX) NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_User_UserRef] ON [employer_account].[User] ([UserRef]) WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_User_Email] ON [employer_account].[User] ([Email]) INCLUDE ([UserRef]) WITH (ONLINE = ON)
GO
