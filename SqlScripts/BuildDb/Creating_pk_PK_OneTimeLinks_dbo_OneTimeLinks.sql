ALTER TABLE [dbo].[OneTimeLinks] ADD CONSTRAINT [PK_OneTimeLinks] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
