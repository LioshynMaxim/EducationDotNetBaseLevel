CREATE TABLE [dbo].[Employees]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[AddressId] INT NOT NULL,
	[PersonId] INT NOT NULL,
	[CompanyName] NVARCHAR(20) NOT NULL,
	[Position] NVARCHAR(30) NULL,
	[EmployeeName] NVARCHAR(100) NULL,
	CONSTRAINT FK_Employees_Address FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Addresses]([Id]),
	CONSTRAINT FK_Employees_Person FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons]([Id])
)
