CREATE NONCLUSTERED INDEX [CONTRIBUTION_FUND_FK_IX] ON [dbo].[Contribution] ([FundId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
