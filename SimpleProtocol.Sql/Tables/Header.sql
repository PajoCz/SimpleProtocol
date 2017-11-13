CREATE TABLE [SimpleProtocol].[Header] (
    [HeaderId]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [CreatedLogin] NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Header] PRIMARY KEY CLUSTERED ([HeaderId] ASC)
);

