ALTER TABLE [dbo].[BundleDetail] ADD CONSTRAINT [BUNDLE_DETAIL_PK] PRIMARY KEY NONCLUSTERED  ([BundleDetailId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
