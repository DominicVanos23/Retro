-- Create the Database
CREATE DATABASE MajokaRentalsDB;
GO

USE MajokaRentalsDB;
GO

/* ===============================
   USERS & ROLES
   =============================== */
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    RoleID INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

/* ===============================
   LISTINGS Table
   =============================== */
-- Listings table
CREATE TABLE Listings (
    ListingID INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    PropertyType NVARCHAR(100),
    Bedrooms INT,
    Bathrooms INT,
    Status NVARCHAR(50)
);

ALTER TABLE Listings
ADD ImageUrls NVARCHAR(MAX) NULL;


-- ListingImages table
CREATE TABLE ListingImages (
    ImageID INT IDENTITY PRIMARY KEY,
    ListingID INT NOT NULL FOREIGN KEY REFERENCES Listings(ListingID),
    ImagePath NVARCHAR(500) NOT NULL
);


/* ===============================
   APPLICATIONS
   =============================== */
CREATE TABLE Applications (
    ApplicationID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    CellNumber NVARCHAR(20),
    EmploymentDetails NVARCHAR(255),
    RentalHistory NVARCHAR(255),
    CertifiedID NVARCHAR(255),
    ProofOfIncome NVARCHAR(255),
    ReferencesDoc NVARCHAR(255),
    SubmissionDate DATETIME DEFAULT GETDATE()
);

ALTER TABLE Applications
ADD Status NVARCHAR(20) DEFAULT 'Pending';



/* ===============================
   MESSAGES (Tenant <-> Manager/Admin)
   =============================== */
CREATE TABLE Messages (
    MessageID INT PRIMARY KEY IDENTITY(1,1),
    SenderID INT NOT NULL,
    ReceiverID INT NOT NULL,
    MessageText NVARCHAR(MAX) NOT NULL,
    SentAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SenderID) REFERENCES Users(UserID),
    FOREIGN KEY (ReceiverID) REFERENCES Users(UserID)
);


/* ===============================
   Maintanance Requests.
   =============================== */

   CREATE TABLE MaintenanceRequests (
    RequestID INT IDENTITY(1,1) PRIMARY KEY,
    TenantName NVARCHAR(100) NOT NULL,
    UnitNumber NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    RequestDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Pending'
);

ALTER TABLE MaintenanceRequests
ADD PhotoPath NVARCHAR(255) NULL,
    AssignedTo NVARCHAR(100) NULL;



Select * From Roles;
Select * From Users;
Select * From Listings;
Select * From ListingImages
Select * From Applications;
Select * From Messages
Select * From MaintenanceRequests

/* Insert statements */
INSERT INTO Roles (RoleName) VALUES ('Tenant'), ('Manager'), ('Admin');


ALTER TABLE Listings
ADD DateAdded DATETIME NOT NULL DEFAULT GETDATE();

ALTER TABLE Listings
ALTER COLUMN ImageUrl NVARCHAR(255) NULL;

ALTER TABLE Listings
ADD 
    PropertyType NVARCHAR(100) NULL,
    Bedrooms INT NULL,
    Bathrooms INT NULL,
    Status NVARCHAR(50) NULL;

Drop Table Listings;

ALTER TABLE Listings
DROP COLUMN ImageUrl;
