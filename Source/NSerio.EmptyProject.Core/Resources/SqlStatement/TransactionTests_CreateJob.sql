INSERT INTO [dbo].[TransactionTests_Job] ([Name]) VALUES (@Name);
DECLARE @JobId int = SCOPE_IDENTITY();
SELECT @JobId AS JobId;