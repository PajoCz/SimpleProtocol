CREATE TABLE [SimpleProtocol].[Detail] (
    [DetailId]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [HeaderId]     BIGINT         NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [CreatedLogin] NVARCHAR (50)  NULL,
    [StatusId]     INT            NOT NULL,
    [Text]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Detail] PRIMARY KEY CLUSTERED ([DetailId] ASC),
    CONSTRAINT [FK_Detail_Header] FOREIGN KEY ([HeaderId]) REFERENCES [SimpleProtocol].[Header] ([HeaderId]),
    CONSTRAINT [FK_Detail_Status] FOREIGN KEY ([StatusId]) REFERENCES [SimpleProtocol].[Status] ([StatusId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Detail]
    ON [SimpleProtocol].[Detail]([DetailId] ASC);

