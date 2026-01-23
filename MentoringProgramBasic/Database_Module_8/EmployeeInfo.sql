CREATE VIEW [dbo].[EmployeeInfo]
AS 
SELECT 
	e.[Id] AS EmployeeId,
	COALESCE(e.[EmployeeName], p.[FirstName] + ' ' + p.[LastName]) AS EmployeeFullName,
	a.[ZipCode] + '_' + a.[State] + ', ' + a.[City] + '-' + a.[Street] AS EmployeeFullAddress,
	e.[CompanyName] + '(' + ISNULL(e.[Position], '') + ')' AS EmployeeCompanyInfo
FROM [dbo].[Employees] e
	INNER JOIN [dbo].[Persons] p ON e.[PersonId] = p.[Id]
	INNER JOIN [dbo].[Addresses] a ON e.[AddressId] = a.[Id]
