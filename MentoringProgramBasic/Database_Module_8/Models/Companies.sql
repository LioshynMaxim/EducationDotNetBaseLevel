CREATE TABLE [dbo].[Companies]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(20) NOT NULL,
	[AddressId] INT NOT NULL,
	CONSTRAINT FK_Companies_Address FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Addresses]([Id])
)
