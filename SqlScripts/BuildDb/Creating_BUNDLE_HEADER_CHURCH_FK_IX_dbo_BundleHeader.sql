CREATE NONCLUSTERED INDEX [BUNDLE_HEADER_CHURCH_FK_IX] ON [dbo].[BundleHeader] ([ChurchId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
