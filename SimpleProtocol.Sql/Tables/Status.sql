CREATE TABLE [SimpleProtocol].[Status] (
    [StatusId] INT           NOT NULL,
    [Key]      NVARCHAR (10) NOT NULL,
    [Value]    NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([StatusId] ASC)
);

