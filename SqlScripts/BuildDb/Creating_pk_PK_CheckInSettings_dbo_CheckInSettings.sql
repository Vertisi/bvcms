ALTER TABLE [dbo].[CheckInSettings] ADD CONSTRAINT [PK_CheckInSettings] PRIMARY KEY CLUSTERED  ([id]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
