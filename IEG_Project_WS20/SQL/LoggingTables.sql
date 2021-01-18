CREATE TABLE [dbo].[Logs] (
    [LogID]       INT           IDENTITY (1, 1) NOT NULL,
    [Message]     VARCHAR (255) NULL,
    [CreatedTime] DATETIME2 (7) NULL,
    [Context]     NVARCHAR (50) NULL
);