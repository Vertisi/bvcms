ALTER TABLE [dbo].[CardIdentifiers] ADD CONSTRAINT [PK_CardIdentifiers] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
