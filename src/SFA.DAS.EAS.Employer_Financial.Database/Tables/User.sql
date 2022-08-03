﻿CREATE TABLE [employer_financial].[User]
(
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [UserRef] UNIQUEIDENTIFIER NOT NULL, 
    [Email] NVARCHAR(255) NOT NULL UNIQUE, 
    [FirstName] NVARCHAR(MAX) NULL, 
    [LastName] NVARCHAR(MAX) NULL, 
    [CorrelationId] NVARCHAR(255) NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_User_UserRef] ON [employer_financial].[User] ([UserRef]) WITH (ONLINE = ON)
GO

CREATE NONCLUSTERED INDEX [IX_User_Email] ON [employer_financial].[User] ([Email]) INCLUDE ([UserRef]) WITH (ONLINE = ON)
GO
