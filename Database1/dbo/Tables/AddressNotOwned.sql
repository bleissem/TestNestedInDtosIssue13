CREATE TABLE [dbo].[AddressNotOwned] (
    [Id]             INT            NOT NULL,
    [Address1]       NVARCHAR (MAX) NOT NULL,
    [Address2]       NVARCHAR (MAX) NULL,
    [City]           NVARCHAR (MAX) NULL,
    [StateOrProvice] NVARCHAR (MAX) NULL,
    [PostalCode]     NVARCHAR (MAX) NULL,
    [CountryCode]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AddressNotOwned] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AddressNotOwned] FOREIGN KEY ([Id]) REFERENCES [dbo].[ContactAddress] ([ContactAddressId])
);

