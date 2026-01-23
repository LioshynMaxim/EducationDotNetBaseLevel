CREATE TRIGGER [dbo].[EmployeeTrigger]
ON [dbo].[Employees]
FOR INSERT
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [dbo].[Companies] (Name, AddressId)
	SELECT DISTINCT
		i.CompanyName,
		i.AddressId
	FROM INSERTED i
		LEFT JOIN [dbo].[Companies] c
			ON c.Name = i.CompanyName AND c.AddressId = i.AddressId
	WHERE c.Id IS NULL;
       
END
