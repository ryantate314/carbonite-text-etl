USE [textMessages]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Ryan Tate
-- Create date: 2018/01/02
-- Description:	Merges messages from the staging tables into the main message store.
--
-- Mod History
-- Date			ID		Change
-- 2018/05/08	!1		Modified to match Conversation layout of stock application.
-- 2018/04/13	!2		Add Send Date update for message merging
-- =============================================
ALTER PROCEDURE [dbo].[USP_Merge]
	@backupDate datetime
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @backupKey int;

	DECLARE @fromDirection tinyint = 1;
	DECLARE @toDirection tinyint = 2;

	BEGIN TRANSACTION;

	-- Create backup entry
	INSERT INTO dbo.[Backup]
		(BackupDate, MergeDate)
		VALUES (@backupDate, CURRENT_TIMESTAMP);
	SET @backupKey = scope_identity();

	-- Merge Contacts
	WITH NewAddresses (Number, ContactName)
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
			THEN UPDATE SET A.ContactName = NA.ContactName;

	--Merge Messages
	WITH NewMessages
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
		--WHEN MATCHED THEN --!2
			--UPDATE SET SendDate = NM.SendDate
	;

	--Link contacts to Messages. Only create joins for TO addresses.
	--INSERT INTO dbo.MessageAddress
	--SELECT PM.[Key] MessageKey, PA.[Key] AddressKey
	--	FROM Staging.MessageAddress SMA
	--		JOIN dbo.[Address] PA ON SMA.Number = PA.Number --Convert number to address key
	--		JOIN dbo.[Message] PM ON SMA.MessageId = PM.MessageId --Convert MessageId to message key
	--	WHERE SMA.Direction = @toDirection
	--		AND PM.BackupKey = @backupKey --Only include new messages
	--;

	--Link message FROM contacts.
	UPDATE PM
		SET PM.FromAddressKey = PA.[Key]
	FROM dbo.[Message] PM
		JOIN Staging.MessageAddress SMA ON PM.MessageId = SMA.MessageId
		JOIN dbo.[Address] PA ON SMA.Number = PA.Number
	WHERE SMA.Direction = @fromDirection
		AND PM.BackupKey = @backupKey --Only include new messages
	;

	--Group into conversations
	--Create Conversation entries
	INSERT INTO dbo.Conversation (ComputedKey)
		SELECT DISTINCT ParticipantHash
		FROM Staging.vw_Conversations SC
		LEFT JOIN dbo.Conversation C ON SC.ParticipantHash = C.ComputedKey
		WHERE C.[Key] IS NULL
	;

	--Link messages to conversations
	UPDATE M
		SET M.ConversationKey = C.[Key]
		FROM dbo.Message M
		JOIN Staging.vw_Conversations SC ON M.MessageId = SC.MessageId
		JOIN dbo.Conversation C ON SC.ParticipantHash = C.ComputedKey;

	--Link conversations to contacts
	WITH SampleMessages
	AS
	(
		--Select one message per conversation
		SELECT
			  M.ConversationKey
			, M.MessageId
		FROM
		(
			SELECT    
				  C.[Key] ConversationKey
				, M.MessageId
				, ROW_NUMBER() OVER (Partition BY C.[Key]
					ORDER BY M.SendDate DESC) AS rk
			FROM dbo.Message M
				JOIN dbo.Conversation C ON M.ConversationKey = C.[Key]
		) AS M
		WHERE rk = 1
	),
	ConversationAddresses AS
	(
		SELECT
			    Samples.ConversationKey
			  , A.[Key] AddressKey
		FROM SampleMessages Samples
		JOIN dbo.Message M ON Samples.MessageId = M.MessageId
		JOIN Staging.Message SM ON SM.MessageId = M.MessageId
		JOIN Staging.MessageAddress MA ON M.MessageId = MA.MessageId
		JOIN dbo.Address A ON MA.Number = A.Number
	)
	MERGE INTO dbo.ConversationAddress T
		USING ConversationAddresses CA
		ON T.ConversationKey = CA.ConversationKey AND T.AddressKey = CA.AddressKey
		WHEN NOT MATCHED BY TARGET 
			THEN INSERT (ConversationKey, AddressKey)
			VALUES (CA.ConversationKey, CA.AddressKey);

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
