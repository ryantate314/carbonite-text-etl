USE [textMessages]
GO
/****** Object:  StoredProcedure [dbo].[USP_Truncate]    Script Date: 7/13/2018 3:48:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Ryan Tate
-- Create date: 2018/01/04
-- Description:	Does what it says.
-- =============================================
ALTER PROCEDURE [dbo].[USP_Truncate]
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM dbo.MessageAddress;
	--DBCC CHECKIDENT ('textMessages.dbo.MessageAddress',RESEED, 0);

	DELETE FROM dbo.[ConversationAddress];
	--DBCC CHECKIDENT ('textMessages.dbo.ConversationAddress',RESEED, 0);

	DELETE FROM dbo.Attachment;
	--DBCC CHECKIDENT ('textMessages.dbo.Attachment',RESEED, 0);

    DELETE FROM dbo.[Message];
	--DBCC CHECKIDENT ('textMessages.dbo.Message',RESEED, 0);

	DELETE FROM dbo.[Conversation];
	--DBCC CHECKIDENT ('textMessages.dbo.Conversation',RESEED, 0);

	--DELETE FROM dbo.[Address];
	--DBCC CHECKIDENT ('textMessages.dbo.Address',RESEED, 0);

	DELETE FROM dbo.[Backup];
	--DBCC CHECKIDENT ('textMessages.dbo.Backup',RESEED, 0);

END
