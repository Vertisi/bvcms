ALTER TABLE [dbo].[TagShare] ADD CONSTRAINT [PK_TagShare] PRIMARY KEY CLUSTERED  ([TagId], [PeopleId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
