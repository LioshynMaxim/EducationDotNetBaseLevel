/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Insert sample data into Persons table
IF NOT EXISTS (SELECT 1 FROM [dbo].[Persons])
BEGIN
    INSERT INTO [dbo].[Persons] ([Id], [FirstName], [LastName]) VALUES
    (1, 'John', 'Doe'),
    (2, 'Jane', 'Smith'),
    (3, 'Michael', 'Johnson'),
    (4, 'Emily', 'Williams'),
    (5, 'David', 'Brown');
END

-- Insert sample data into Addresses table
IF NOT EXISTS (SELECT 1 FROM [dbo].[Addresses])
BEGIN
    INSERT INTO [dbo].[Addresses] ([Id], [Street], [City], [State], [ZipCode]) VALUES
    (1, '123 Main St', 'New York', 'NY', '10001'),
    (2, '456 Oak Ave', 'Los Angeles', 'CA', '90001'),
    (3, '789 Pine Rd', 'Chicago', 'IL', '60601'),
    (4, '321 Elm St', 'Houston', 'TX', '77001'),
    (5, '654 Maple Dr', 'Phoenix', 'AZ', '85001'),
    (6, '987 Cedar Ln', 'Seattle', 'WA', '98101'),
    (7, '147 Birch Way', 'Boston', 'MA', '02101');
END

-- Insert sample data into Companies table
IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies])
BEGIN
    INSERT INTO [dbo].[Companies] ([Id], [Name], [AddressId]) VALUES
    (1, 'Tech Corp', 1),
    (2, 'Software Inc', 2),
    (3, 'Data Systems', 3);
END

-- Insert sample data into Employees table
IF NOT EXISTS (SELECT 1 FROM [dbo].[Employees])
BEGIN
    INSERT INTO [dbo].[Employees] ([Id], [AddressId], [PersonId], [CompanyName], [Position], [EmployeeName]) VALUES
    (1, 4, 1, 'Tech Corp', 'Software Engineer', 'John Doe'),
    (2, 5, 2, 'Software Inc', 'Project Manager', 'Jane Smith'),
    (3, 6, 3, 'Data Systems', 'Database Admin', 'Michael Johnson'),
    (4, 7, 4, 'Tech Corp', 'UI/UX Designer', 'Emily Williams'),
    (5, 4, 5, 'Software Inc', 'DevOps Engineer', 'David Brown');
END
