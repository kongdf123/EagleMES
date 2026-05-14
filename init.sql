create database EagleMes;

GO

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE WorkOrders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderNo NVARCHAR(50) NOT NULL,
    ProductCode NVARCHAR(50) NOT NULL,
    Quantity INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    StartTime DATETIME NULL,
    EndTime DATETIME NULL
);

CREATE TABLE Inventories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MaterialCode NVARCHAR(50) NOT NULL,
    Quantity INT NOT NULL,
    Location NVARCHAR(50) NOT NULL
);


CREATE TABLE Devices (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DeviceCode NVARCHAR(50) NOT NULL,
    DeviceName NVARCHAR(100) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Temperature FLOAT NOT NULL,
    LastUpdated DATETIME NOT NULL
);

CREATE TABLE EventLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EventType NVARCHAR(50),
    Message NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE()
);

INSERT INTO WorkOrders (OrderNo, ProductCode, Quantity, Status)
VALUES
('WO-1001', 'P-AX01', 100, 'Pending'),
('WO-1002', 'P-BX02', 200, 'Running');

INSERT INTO Users (Username, PasswordHash, Role)
VALUES ('admin', 'admin123', 'Admin');

INSERT INTO Inventories (MaterialCode, Quantity, Location)
VALUES
('MAT-001', 500, 'WH-A'),
('MAT-002', 800, 'WH-B');

INSERT INTO Devices (DeviceCode, DeviceName, Status, Temperature, LastUpdated)
VALUES
('PLC-01', 'Assembly Line PLC', 'Running', 36.5, GETDATE()),
('PLC-02', 'Packing PLC', 'Running', 34.2, GETDATE()),
('SCN-01', 'Barcode Scanner', 'Idle', 29.1, GETDATE()),
('RFID-01', 'RFID Gateway', 'Running', 31.8, GETDATE());