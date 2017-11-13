CREATE TABLE [SimpleProtocol].[LinkedObject] (
    [LinkedObjectId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [HeaderId]       BIGINT         NOT NULL,
    [Name]           NVARCHAR (250) NOT NULL,
    [Id]             NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LinkedObject] PRIMARY KEY CLUSTERED ([LinkedObjectId] ASC),
    CONSTRAINT [FK_LinkedObject_Header] FOREIGN KEY ([HeaderId]) REFERENCES [SimpleProtocol].[Header] ([HeaderId])
);

