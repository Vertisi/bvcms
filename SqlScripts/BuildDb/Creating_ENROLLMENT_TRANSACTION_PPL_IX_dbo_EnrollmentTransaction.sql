CREATE NONCLUSTERED INDEX [ENROLLMENT_TRANSACTION_PPL_IX] ON [dbo].[EnrollmentTransaction] ([PeopleId]) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
