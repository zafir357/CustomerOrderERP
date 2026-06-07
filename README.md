# ErpDemo

A simple Windows Forms desktop application for managing customers and their orders, built with .NET 10 and SQL Server.

## What it does

- **Customers** — view, add, and delete customers
- **Orders** — view, add, and delete orders per customer
- Selecting a customer in the left panel automatically loads their orders in the right panel
- Deleting a customer also deletes all their orders (handled in a SQL transaction)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance, e.g. SQL Server Express)

## Database setup

Run the following SQL against your SQL Server instance to create the database and required objects:

```sql
CREATE DATABASE ErpDemo;
GO

USE ErpDemo;
GO

CREATE TABLE Customers (
    Id        INT IDENTITY PRIMARY KEY,
    Name      NVARCHAR(150) NOT NULL,
    Email     NVARCHAR(200) NULL,
    Phone     NVARCHAR(50)  NULL,
    CreatedAt DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Orders (
    Id         INT IDENTITY PRIMARY KEY,
    CustomerId INT            NOT NULL REFERENCES Customers(Id),
    Reference  NVARCHAR(100)  NOT NULL,
    Amount     DECIMAL(18, 2) NOT NULL,
    Status     NVARCHAR(50)   NOT NULL DEFAULT 'Pending',
    OrderDate  DATETIME       NOT NULL DEFAULT GETDATE()
);

CREATE PROCEDURE sp_GetOrdersByCustomer
    @CustomerId INT
AS
BEGIN
    SELECT Id, Reference, Amount, Status, OrderDate
    FROM   Orders
    WHERE  CustomerId = @CustomerId
    ORDER  BY OrderDate DESC;
END;
```

## Connection string

The connection string is defined in `ErpDemo/DbHelper.cs`:

```csharp
public const string ConnStr =
    "Server=localhost;Database=ErpDemo;" +
    "Trusted_Connection=True;TrustServerCertificate=True;";
```

Edit this string to match your environment:

| Scenario | Example value |
|---|---|
| Named instance | `Server=localhost\SQLEXPRESS` |
| Custom port | `Server=localhost,1433` |
| SQL authentication | Replace `Trusted_Connection=True` with `User Id=sa;Password=yourpwd` |

## Run

```
dotnet run --project ErpDemo
```

Or open `ErpDemo.slnx` in Visual Studio 2022+ and press **F5**.
