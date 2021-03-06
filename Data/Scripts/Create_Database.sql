USE [textMessages]
GO
/****** Object:  User [textMessageAdmin]    Script Date: 1/11/2018 12:45:18 PM ******/
CREATE USER [textMessageAdmin] FOR LOGIN [textMessageAdmin] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [textMessageProcessor]    Script Date: 1/11/2018 12:45:18 PM ******/
CREATE USER [textMessageProcessor] FOR LOGIN [textMessageProcessor] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [textMessageAdmin]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [textMessageAdmin]
GO
ALTER ROLE [db_datareader] ADD MEMBER [textMessageAdmin]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [textMessageAdmin]
GO
ALTER ROLE [db_datareader] ADD MEMBER [textMessageProcessor]
GO
/****** Object:  Schema [Staging]    Script Date: 1/11/2018 12:45:18 PM ******/
CREATE SCHEMA [Staging]
GO
/****** Object:  UserDefinedTableType [dbo].[ContactInfo]    Script Date: 1/11/2018 12:45:18 PM ******/
CREATE TYPE [dbo].[ContactInfo] AS TABLE(
	[Number] [varchar](128) NULL,
	[NewName] [varchar](128) NULL
)
GO
/****** Object:  Table [dbo].[Message]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Message](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[MessageType] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[MessageId] [varchar](32) NOT NULL,
	[FromAddressKey] [int] NULL,
	[BackupKey] [int] NOT NULL,
 CONSTRAINT [PK_Message_1] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attachment]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachment](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](128) NOT NULL,
	[Path] [varchar](256) NOT NULL,
	[MimeType] [varchar](128) NOT NULL,
	[MessageKey] [int] NOT NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[Number] [varchar](128) NULL,
	[ContactName] [varchar](128) NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageAddress]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageAddress](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[MessageKey] [int] NOT NULL,
	[AddressKey] [int] NOT NULL,
 CONSTRAINT [PK_MessageAddress] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageType]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageType](
	[Key] [int] NOT NULL,
	[Description] [varchar](32) NOT NULL,
 CONSTRAINT [PK_MessageType] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[VW_Conversations]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[VW_Conversations]
AS (
	SELECT 
		M.[Key],
		M.SendDate,
		M.Body,
		MT.[Description] MessageType,
		A.ContactName,
		ISNULL(AC.NumAttachments, 0) NumAttachments
	FROM dbo.[Message] M
		JOIN dbo.MessageAddress MA ON M.[Key] = MA.MessageKey
		JOIN dbo.[Address] A ON MA.AddressKey = A.[Key]
		LEFT JOIN (
			SELECT A.[MessageKey], COUNT(1) NumAttachments
			FROM dbo.Attachment A
			GROUP BY A.[MessageKey]
		) AC ON M.[Key] = AC.MessageKey
		JOIN dbo.MessageType MT ON M.MessageType = MT.[Key]

	UNION ALL

	SELECT 
		M.[Key],
		M.SendDate,
		M.Body,
		MT.[Description] MessageType,
		A.ContactName,
		ISNULL(AC.NumAttachments, 0) NumAttachments
	FROM dbo.[Message] M
		JOIN dbo.[Address] A ON M.FromAddressKey = A.[Key]
		LEFT JOIN (
			SELECT A.[MessageKey], COUNT(1) NumAttachments
			FROM dbo.Attachment A
			GROUP BY A.[MessageKey]
		) AC ON M.[Key] = AC.MessageKey
		JOIN dbo.MessageType MT ON M.MessageType = MT.[Key]
);
GO
/****** Object:  Table [dbo].[Backup]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Backup](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[BatchDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Backup] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Staging].[AddressDirection]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Staging].[AddressDirection](
	[Key] [int] NOT NULL,
	[Description] [varchar](32) NOT NULL,
 CONSTRAINT [PK_AddressDirection] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Staging].[Attachment]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Staging].[Attachment](
	[FileName] [varchar](128) NOT NULL,
	[Path] [varchar](256) NOT NULL,
	[MimeType] [varchar](128) NOT NULL,
	[MessageId] [varchar](32) NOT NULL,
 CONSTRAINT [PK_Attachment_1] PRIMARY KEY CLUSTERED 
(
	[Path] ASC,
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Staging].[Message]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Staging].[Message](
	[MessageId] [varchar](32) NOT NULL,
	[SendDate] [datetime] NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[MessageType] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Staging].[MessageAddress]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Staging].[MessageAddress](
	[MessageId] [varchar](32) NOT NULL,
	[Number] [varchar](128) NOT NULL,
	[ContactName] [varchar](64) NULL,
	[Direction] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC,
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Attachment]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentMessageKey_MessageKey] FOREIGN KEY([MessageKey])
REFERENCES [dbo].[Message] ([Key])
GO
ALTER TABLE [dbo].[Attachment] CHECK CONSTRAINT [FK_AttachmentMessageKey_MessageKey]
GO
ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_MessageBackupKey_backup_Key] FOREIGN KEY([BackupKey])
REFERENCES [dbo].[Backup] ([Key])
GO
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_MessageBackupKey_backup_Key]
GO
ALTER TABLE [dbo].[MessageAddress]  WITH CHECK ADD  CONSTRAINT [FK_MessageAddressAddressKey_AddressKey] FOREIGN KEY([AddressKey])
REFERENCES [dbo].[Address] ([Key])
GO
ALTER TABLE [dbo].[MessageAddress] CHECK CONSTRAINT [FK_MessageAddressAddressKey_AddressKey]
GO
ALTER TABLE [dbo].[MessageAddress]  WITH CHECK ADD  CONSTRAINT [FK_MessageAddressMessageKey_MessageKey] FOREIGN KEY([MessageKey])
REFERENCES [dbo].[Message] ([Key])
GO
ALTER TABLE [dbo].[MessageAddress] CHECK CONSTRAINT [FK_MessageAddressMessageKey_MessageKey]
GO
/****** Object:  StoredProcedure [dbo].[USP_Count_Messages]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[USP_Count_Messages]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT C.ContactName, COUNT(1) NumMessages
	FROM dbo.[VW_Conversations] C
	GROUP BY C.ContactName
	ORDER BY NumMessages DESC
END
GO
/****** Object:  StoredProcedure [dbo].[USP_Merge]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ryan Tate
-- Create date: 2018/01/02
-- Description:	Merges messages from the staging tables into the main message store.
-- =============================================
CREATE PROCEDURE [dbo].[USP_Merge]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @backupKey int;

	DECLARE @fromDirection tinyint = 1;
	DECLARE @toDirection tinyint = 2;

	BEGIN TRANSACTION;

	-- Create backup entry
	INSERT INTO dbo.[Backup]
		(BatchDate)
		VALUES (CURRENT_TIMESTAMP);
	SET @backupKey = scope_identity();

	-- Merge Contacts
	;WITH NewAddresses (Number, ContactName)
	AS
	(
		SELECT DISTINCT Number, ContactName
		FROM Staging.[MessageAddress] SMA
	)
	MERGE dbo.[Address] A
		USING NewAddresses NA
		ON A.Number = NA.Number
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (Number, ContactName)
			VALUES (NA.Number, NA.ContactName)
		WHEN MATCHED
			THEN UPDATE SET A.ContactName = NA.ContactName
	;

	--Merge Messages
	;WITH NewMessages
	AS
	(
		SELECT MessageId, SendDate, Body, MessageType, [Status]
		FROM Staging.[Message] M
	)
	MERGE dbo.[Message] M
		USING NewMessages NM
		ON M.MessageId = NM.MessageId
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (MessageId, SendDate, Body, MessageType, [Status], BackupKey)
			VALUES (NM.MessageId, NM.SendDate, NM.Body, NM.MessageType, NM.[Status], @backupKey)
	;

	--Link contacts to Messages. Only create joins for TO addresses.
	INSERT INTO dbo.MessageAddress
	SELECT PM.[Key] MessageKey, PA.[Key] AddressKey
		FROM Staging.MessageAddress SMA
			JOIN dbo.[Address] PA ON SMA.Number = PA.Number --Convert number to address key
			JOIN dbo.[Message] PM ON SMA.MessageId = PM.MessageId --Convert MessageId to message key
		WHERE SMA.Direction = @toDirection
			AND PM.BackupKey = @backupKey --Only include new messages
	;

	--Link message FROM contacts.
	UPDATE PM
		SET PM.FromAddressKey = PA.[Key]
	FROM dbo.[Message] PM
		JOIN Staging.MessageAddress SMA ON PM.MessageId = SMA.MessageId
		JOIN dbo.[Address] PA ON SMA.Number = PA.Number
	WHERE SMA.Direction = @fromDirection
		AND PM.BackupKey = @backupKey --Only include new messages
	;

	-- Merge Attachments
	INSERT INTO dbo.Attachment
		SELECT SA.[FileName], SA.[Path], SA.MimeType, PM.[Key] MessageKey
		FROM Staging.Attachment SA
			JOIN dbo.[Message] PM ON SA.MessageId = PM.MessageId --Get message key
			LEFT JOIN dbo.[Attachment] PA ON PM.[Key] = PA.MessageKey
			WHERE PM.BackupKey = @backupKey --Only include new attachments
	;
    
	COMMIT TRANSACTION;
END
GO
/****** Object:  StoredProcedure [dbo].[USP_Truncate]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Ryan Tate
-- Create date: 2018/01/04
-- Description:	Does what it says.
-- =============================================
CREATE PROCEDURE [dbo].[USP_Truncate]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM dbo.MessageAddress;
	DBCC CHECKIDENT ('textMessages.dbo.MessageAddress',RESEED, 0);

	DELETE FROM dbo.Attachment;
	DBCC CHECKIDENT ('textMessages.dbo.Attachment',RESEED, 0);

    DELETE FROM dbo.[Message];
	DBCC CHECKIDENT ('textMessages.dbo.Message',RESEED, 0);

	DELETE FROM dbo.[Address];
	DBCC CHECKIDENT ('textMessages.dbo.Address',RESEED, 0);

	DELETE FROM dbo.[Backup];
	DBCC CHECKIDENT ('textMessages.dbo.Backup',RESEED, 0);

END
GO
/****** Object:  StoredProcedure [Staging].[USP_Select_Contact_Options]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ryan Tate
-- Create date: 2018/01/08
-- Description:	This proc builds a list of possible contact names
--    and removes any unnessary empty ones.
-- =============================================
CREATE PROCEDURE [Staging].[USP_Select_Contact_Options]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	WITH ContactOptions AS 
    (
		SELECT DISTINCT Number, ContactName
		FROM Staging.[MessageAddress]
		UNION
		SELECT Number, ContactName
		FROM dbo.[Address]
	)
	SELECT * FROM ContactOptions CO
	WHERE
		( ContactName != '' AND ContactName IS NOT NULL )
		OR
		NOT EXISTS (
			SELECT * FROM ContactOptions CO2
			WHERE CO2.Number = CO.Number
			AND CO2.ContactName != '' AND ContactName IS NOT NULL
		)
	ORDER BY CO.Number;
END
GO
/****** Object:  StoredProcedure [Staging].[USP_Truncate_Staging]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Staging].[USP_Truncate_Staging]
WITH EXECUTE AS 'textMessageAdmin'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    TRUNCATE TABLE Staging.[Message];
	TRUNCATE TABLE Staging.Attachment;
	TRUNCATE TABLE Staging.MessageAddress;

END
GO
/****** Object:  StoredProcedure [Staging].[USP_Update_Contacts]    Script Date: 1/11/2018 12:45:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Ryan Tate
-- Create date: 2018/01/08
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Staging].[USP_Update_Contacts]
	@contactInfo ContactInfo READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE Staging.MessageAddress
	SET ContactName = CI.NewName
	FROM @contactInfo CI
	WHERE Staging.MessageAddress.Number = CI.Number;
	
	--This will be updated using USP_Merge
	--UPDATE dbo.[Address]
	--SET ContactName = @newName
	--WHERE Number = @number;
END
GO
