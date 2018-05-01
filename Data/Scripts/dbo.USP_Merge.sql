USE [textMessages]
GO

/****** Object:  StoredProcedure [dbo].[USP_Merge]    Script Date: 1/5/2018 8:54:56 AM ******/
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

