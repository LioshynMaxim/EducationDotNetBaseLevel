CREATE PROCEDURE [dbo].[AddEmployee]
    @EmployeeName NVARCHAR(100) = NULL,
    @FirstName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50) = NULL,
    @CompanyName NVARCHAR(20),
    @Position NVARCHAR(30) = NULL,
    @Street NVARCHAR(50),
    @City NVARCHAR(20) = NULL,
    @State NVARCHAR(50) = NULL,
    @ZipCode NVARCHAR(50) = NULL
AS
BEGIN
    IF (ISNULL(LTRIM(RTRIM(@EmployeeName)), '') = '' OR
        ISNULL(LTRIM(RTRIM(@FirstName)), '') = '' OR
        ISNULL(LTRIM(RTRIM(@LastName)), '') = '')
    BEGIN
        RAISERROR('Wrong EmployeeName or FirstName or LastName.', 16, 1);
        RETURN;
    END

    SET @CompanyName = SUBSTRING(@CompanyName, 1, 20);

    DECLARE @AddressId INT;
    INSERT INTO [dbo].[Addresses]
        ([Street], [City], [State], [ZipCode])
    VALUES (@Street, @City, @State, @ZipCode);
    SET @AddressId = SCOPE_IDENTITY();

    DECLARE @PersonId INT;
    INSERT INTO [dbo].[Persons]
        ([FirstName], [LastName])
    VALUES (ISNULL(@FirstName, @EmployeeName), ISNULL(@LastName, @EmployeeName));
    SET @PersonId = SCOPE_IDENTITY();

    INSERT INTO [dbo].[Employees]
        ([AddressId], [PersonId], [CompanyName], [Position], [EmployeeName])
    VALUES (@AddressId, @PersonId, @CompanyName, @Position, @EmployeeName);
END