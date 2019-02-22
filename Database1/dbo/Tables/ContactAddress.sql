CREATE TABLE [dbo].[ContactAddress] (
    [ContactAddressId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ContactAddress] PRIMARY KEY CLUSTERED ([ContactAddressId] ASC)
);

