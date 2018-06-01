
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/22/2018 13:34:26
-- Generated from EDMX file: C:\inetpub\wwwroot\DebtorsControl\DebtorsControl\Models\DebtorsModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [pdInvoice];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Dollars_Clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Dollars] DROP CONSTRAINT [FK_Dollars_Clients];
GO
IF OBJECT_ID(N'[dbo].[FK_Dollars_Invoices]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Dollars] DROP CONSTRAINT [FK_Dollars_Invoices];
GO
IF OBJECT_ID(N'[dbo].[FK_Invoices_Clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoices] DROP CONSTRAINT [FK_Invoices_Clients];
GO
IF OBJECT_ID(N'[dbo].[FK_Nairas_Clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Nairas] DROP CONSTRAINT [FK_Nairas_Clients];
GO
IF OBJECT_ID(N'[dbo].[FK_Nairas_Invoices]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Nairas] DROP CONSTRAINT [FK_Nairas_Invoices];
GO
IF OBJECT_ID(N'[dbo].[FK_Reports_Clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reports] DROP CONSTRAINT [FK_Reports_Clients];
GO
IF OBJECT_ID(N'[dbo].[FK_Reports_Invoices]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reports] DROP CONSTRAINT [FK_Reports_Invoices];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceEnteries_Clients]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceEnteries] DROP CONSTRAINT [FK_ServiceEnteries_Clients];
GO
IF OBJECT_ID(N'[dbo].[FK_ServiceEnteries_Invoices]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ServiceEnteries] DROP CONSTRAINT [FK_ServiceEnteries_Invoices];
GO
IF OBJECT_ID(N'[dbo].[FK_Staffs_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Staffs] DROP CONSTRAINT [FK_Staffs_Roles];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ActivityLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityLog];
GO
IF OBJECT_ID(N'[dbo].[Clients]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clients];
GO
IF OBJECT_ID(N'[dbo].[Dollars]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Dollars];
GO
IF OBJECT_ID(N'[dbo].[Invoices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Invoices];
GO
IF OBJECT_ID(N'[dbo].[Nairas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Nairas];
GO
IF OBJECT_ID(N'[dbo].[Reports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reports];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[ServiceEnteries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ServiceEnteries];
GO
IF OBJECT_ID(N'[dbo].[Staffs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Staffs];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ActivityLogs'
CREATE TABLE [dbo].[ActivityLogs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Staffinfo] nvarchar(max)  NOT NULL,
    [TypeofActivity] varchar(max)  NOT NULL,
    [TimeCreated] nvarchar(max)  NULL
);
GO

-- Creating table 'Clients'
CREATE TABLE [dbo].[Clients] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ClientName] nvarchar(400)  NOT NULL,
    [ClientCode] nvarchar(400)  NOT NULL,
    [ClientLogo] varchar(max)  NULL
);
GO

-- Creating table 'Dollars'
CREATE TABLE [dbo].[Dollars] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ClientName] nvarchar(400)  NOT NULL,
    [Year] int  NOT NULL,
    [Month] int  NOT NULL,
    [InvoiceNumber] nvarchar(256)  NOT NULL,
    [SENumber] nvarchar(256)  NULL,
    [Amount] decimal(18,5)  NOT NULL,
    [VAT] decimal(18,5)  NOT NULL,
    [TotalInvoice] decimal(18,5)  NOT NULL,
    [Payable] decimal(18,5)  NOT NULL,
    [LcdCharge] decimal(18,5)  NOT NULL,
    [AmountPaid] decimal(18,5)  NOT NULL,
    [Outstanding] decimal(18,5)  NOT NULL,
    [DateSubmitted] datetime  NOT NULL,
    [DueDate] datetime  NOT NULL,
    [DatePaid] datetime  NULL,
    [WithHoldinTax] decimal(18,5)  NOT NULL,
    [RemittanceAdvise] nvarchar(max)  NULL,
    [Comments] varchar(max)  NULL
);
GO

-- Creating table 'Invoices'
CREATE TABLE [dbo].[Invoices] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [InvoiceNumber] nvarchar(256)  NOT NULL,
    [ClientName] nvarchar(400)  NOT NULL
);
GO

-- Creating table 'Nairas'
CREATE TABLE [dbo].[Nairas] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ClientName] nvarchar(400)  NOT NULL,
    [Year] int  NULL,
    [Month] int  NULL,
    [InvoiceNumber] nvarchar(256)  NOT NULL,
    [SENumber] nvarchar(256)  NULL,
    [FxRate] decimal(18,5)  NULL,
    [Amount] decimal(18,5)  NOT NULL,
    [VAT] decimal(18,5)  NOT NULL,
    [TotalInvoice] decimal(18,5)  NOT NULL,
    [Payable] decimal(18,5)  NOT NULL,
    [LcdCharge] decimal(18,5)  NOT NULL,
    [AmountPaid] decimal(18,5)  NOT NULL,
    [Outstanding] decimal(18,5)  NOT NULL,
    [DateSubmitted] datetime  NOT NULL,
    [DueDate] datetime  NOT NULL,
    [DatePaid] datetime  NULL,
    [WithHoldingTax] decimal(18,5)  NOT NULL,
    [RemittanceAdvise] nvarchar(max)  NULL,
    [Comments] nvarchar(max)  NULL,
    [NairaValue] decimal(18,5)  NULL
);
GO

-- Creating table 'Reports'
CREATE TABLE [dbo].[Reports] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ClientName] nvarchar(400)  NOT NULL,
    [Year] int  NOT NULL,
    [Month] int  NOT NULL,
    [InvoiceNumber] nvarchar(256)  NOT NULL,
    [SENumber] nvarchar(256)  NOT NULL,
    [Amount] decimal(18,0)  NOT NULL,
    [AmountPaid] decimal(18,0)  NOT NULL,
    [Outstanding] decimal(18,0)  NOT NULL,
    [DatePaid] datetime  NOT NULL,
    [WithHoldingTaxStatus] bit  NOT NULL,
    [RemittanceAdviseStatus] bit  NOT NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Role1] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'ServiceEnteries'
CREATE TABLE [dbo].[ServiceEnteries] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [SENumber] nvarchar(256)  NULL,
    [InvoiceNumber] nvarchar(256)  NOT NULL,
    [ClientName] nvarchar(400)  NOT NULL
);
GO

-- Creating table 'Staffs'
CREATE TABLE [dbo].[Staffs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Email] nvarchar(256)  NOT NULL,
    [FullName] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Role] nvarchar(256)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'ActivityLogs'
ALTER TABLE [dbo].[ActivityLogs]
ADD CONSTRAINT [PK_ActivityLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ClientName] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [PK_Clients]
    PRIMARY KEY CLUSTERED ([ClientName] ASC);
GO

-- Creating primary key on [Id] in table 'Dollars'
ALTER TABLE [dbo].[Dollars]
ADD CONSTRAINT [PK_Dollars]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [InvoiceNumber] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [PK_Invoices]
    PRIMARY KEY CLUSTERED ([InvoiceNumber] ASC);
GO

-- Creating primary key on [Id] in table 'Nairas'
ALTER TABLE [dbo].[Nairas]
ADD CONSTRAINT [PK_Nairas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [PK_Reports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Role1] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Role1] ASC);
GO

-- Creating primary key on [Id] in table 'ServiceEnteries'
ALTER TABLE [dbo].[ServiceEnteries]
ADD CONSTRAINT [PK_ServiceEnteries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Email] in table 'Staffs'
ALTER TABLE [dbo].[Staffs]
ADD CONSTRAINT [PK_Staffs]
    PRIMARY KEY CLUSTERED ([Email] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ClientName] in table 'Dollars'
ALTER TABLE [dbo].[Dollars]
ADD CONSTRAINT [FK_Dollars_Clients]
    FOREIGN KEY ([ClientName])
    REFERENCES [dbo].[Clients]
        ([ClientName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Dollars_Clients'
CREATE INDEX [IX_FK_Dollars_Clients]
ON [dbo].[Dollars]
    ([ClientName]);
GO

-- Creating foreign key on [ClientName] in table 'Invoices'
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT [FK_Invoices_Clients]
    FOREIGN KEY ([ClientName])
    REFERENCES [dbo].[Clients]
        ([ClientName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Invoices_Clients'
CREATE INDEX [IX_FK_Invoices_Clients]
ON [dbo].[Invoices]
    ([ClientName]);
GO

-- Creating foreign key on [ClientName] in table 'Nairas'
ALTER TABLE [dbo].[Nairas]
ADD CONSTRAINT [FK_Nairas_Clients]
    FOREIGN KEY ([ClientName])
    REFERENCES [dbo].[Clients]
        ([ClientName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Nairas_Clients'
CREATE INDEX [IX_FK_Nairas_Clients]
ON [dbo].[Nairas]
    ([ClientName]);
GO

-- Creating foreign key on [ClientName] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [FK_Reports_Clients]
    FOREIGN KEY ([ClientName])
    REFERENCES [dbo].[Clients]
        ([ClientName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reports_Clients'
CREATE INDEX [IX_FK_Reports_Clients]
ON [dbo].[Reports]
    ([ClientName]);
GO

-- Creating foreign key on [ClientName] in table 'ServiceEnteries'
ALTER TABLE [dbo].[ServiceEnteries]
ADD CONSTRAINT [FK_ServiceEnteries_Clients]
    FOREIGN KEY ([ClientName])
    REFERENCES [dbo].[Clients]
        ([ClientName])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceEnteries_Clients'
CREATE INDEX [IX_FK_ServiceEnteries_Clients]
ON [dbo].[ServiceEnteries]
    ([ClientName]);
GO

-- Creating foreign key on [InvoiceNumber] in table 'Dollars'
ALTER TABLE [dbo].[Dollars]
ADD CONSTRAINT [FK_Dollars_Invoices]
    FOREIGN KEY ([InvoiceNumber])
    REFERENCES [dbo].[Invoices]
        ([InvoiceNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Dollars_Invoices'
CREATE INDEX [IX_FK_Dollars_Invoices]
ON [dbo].[Dollars]
    ([InvoiceNumber]);
GO

-- Creating foreign key on [InvoiceNumber] in table 'Nairas'
ALTER TABLE [dbo].[Nairas]
ADD CONSTRAINT [FK_Nairas_Invoices]
    FOREIGN KEY ([InvoiceNumber])
    REFERENCES [dbo].[Invoices]
        ([InvoiceNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Nairas_Invoices'
CREATE INDEX [IX_FK_Nairas_Invoices]
ON [dbo].[Nairas]
    ([InvoiceNumber]);
GO

-- Creating foreign key on [InvoiceNumber] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [FK_Reports_Invoices]
    FOREIGN KEY ([InvoiceNumber])
    REFERENCES [dbo].[Invoices]
        ([InvoiceNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Reports_Invoices'
CREATE INDEX [IX_FK_Reports_Invoices]
ON [dbo].[Reports]
    ([InvoiceNumber]);
GO

-- Creating foreign key on [InvoiceNumber] in table 'ServiceEnteries'
ALTER TABLE [dbo].[ServiceEnteries]
ADD CONSTRAINT [FK_ServiceEnteries_Invoices]
    FOREIGN KEY ([InvoiceNumber])
    REFERENCES [dbo].[Invoices]
        ([InvoiceNumber])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ServiceEnteries_Invoices'
CREATE INDEX [IX_FK_ServiceEnteries_Invoices]
ON [dbo].[ServiceEnteries]
    ([InvoiceNumber]);
GO

-- Creating foreign key on [Role] in table 'Staffs'
ALTER TABLE [dbo].[Staffs]
ADD CONSTRAINT [FK_Staffs_Roles]
    FOREIGN KEY ([Role])
    REFERENCES [dbo].[Roles]
        ([Role1])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Staffs_Roles'
CREATE INDEX [IX_FK_Staffs_Roles]
ON [dbo].[Staffs]
    ([Role]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------