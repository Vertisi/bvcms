ALTER TABLE [dbo].[SMSItems] ADD CONSTRAINT [PK_SMSItems] PRIMARY KEY CLUSTERED  ([ID]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
