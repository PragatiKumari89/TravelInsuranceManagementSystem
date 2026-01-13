SET IDENTITY_INSERT [dbo].[Users] ON
INSERT INTO [dbo].[Users] ([Id], [FullName], [Email], [Password], [Role], [ResetToken], [ResetTokenExpiry]) VALUES (1, N'JS', N'joyashamim55@gmail.com', N'$2a$11$SE9AOKFzwD2P3AKJCLXs/uBZV5io4O5yUrYp73ggySoGPVY5A2ltK', N'User', NULL, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
