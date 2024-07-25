CREATE TABLE [dbo].[TransactionTests_Job]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    CONSTRAINT [PK_TransactionTests_Job] PRIMARY KEY 
    (
        [Id] ASC
    )
);

CREATE TABLE [dbo].[TransactionTests_Transactions]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [JobId] [int] NOT NULL,
    [Name] [nvarchar](max) NULL,
    CONSTRAINT [PK_TransactionTests_Transactions] PRIMARY KEY 
    (
        [Id] ASC
    ),
    CONSTRAINT [FK_TransactionTests_Transactions_TransactionTests_Job_JobId] FOREIGN KEY 
    (
        [JobId]
    ) REFERENCES [dbo].[TransactionTests_Job] 
    (
        [Id]
    )
);