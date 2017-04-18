CREATE PROCEDURE [employer_account].[UpsertUser]
	@userRef uniqueidentifier,
	@email nvarchar(255),
	@firstName nvarchar(max),
	@lastName nvarchar(max)
AS
	MERGE [employer_account].[User] AS [Target]
	USING (SELECT @userRef AS UserRef) AS [Source] 
	ON [Target].UserRef = [Source].UserRef
	WHEN MATCHED THEN  UPDATE SET [Target].Email = @email, [Target].FirstName = @firstname, [Target].LastName = @lastname
	WHEN NOT MATCHED THEN  INSERT (UserRef, Email, FirstName, LastName) VALUES (@userRef, @email, @firstname, @lastname);
