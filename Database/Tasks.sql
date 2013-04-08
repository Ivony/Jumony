CREATE TABLE [dbo].[Tasks] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Title]     NVARCHAR (200) NOT NULL,
    [Completed] BIT            CONSTRAINT [DF_Tasks_Completed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED ([ID] ASC)
);

