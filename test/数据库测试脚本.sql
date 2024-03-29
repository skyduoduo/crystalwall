SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[permission]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[permission](
	[id] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_permission_id]  DEFAULT (newid()),
	[name] [varchar](500) NULL,
	[action] [varchar](500) NULL,
	[class] [varchar](500) NOT NULL,
 CONSTRAINT [PK_permission] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[principal]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[principal](
	[id] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_user_id]  DEFAULT (newid()),
	[pname] [varchar](50) NOT NULL,
	[password] [varchar](50) NULL,
 CONSTRAINT [PK_user] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[principal_permission]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[principal_permission](
	[id] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_user_permission_id]  DEFAULT (newid()),
	[principal_id] [uniqueidentifier] NOT NULL,
	[permission_id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_user_permission] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_user_permission_permission]') AND parent_object_id = OBJECT_ID(N'[dbo].[principal_permission]'))
ALTER TABLE [dbo].[principal_permission]  WITH CHECK ADD  CONSTRAINT [FK_user_permission_permission] FOREIGN KEY([permission_id])
REFERENCES [dbo].[permission] ([id])
GO
ALTER TABLE [dbo].[principal_permission] CHECK CONSTRAINT [FK_user_permission_permission]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_user_permission_user]') AND parent_object_id = OBJECT_ID(N'[dbo].[principal_permission]'))
ALTER TABLE [dbo].[principal_permission]  WITH CHECK ADD  CONSTRAINT [FK_user_permission_user] FOREIGN KEY([principal_id])
REFERENCES [dbo].[principal] ([id])
GO
ALTER TABLE [dbo].[principal_permission] CHECK CONSTRAINT [FK_user_permission_user]
