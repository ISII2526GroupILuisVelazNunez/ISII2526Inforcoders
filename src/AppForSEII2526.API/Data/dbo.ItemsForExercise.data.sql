SET IDENTITY_INSERT [dbo].[Brands] ON
INSERT INTO [dbo].[Brands] ([Id], [Name]) VALUES (1, N'AaaA')
INSERT INTO [dbo].[Brands] ([Id], [Name]) VALUES (2, N'BbbB')
SET IDENTITY_INSERT [dbo].[Brands] OFF

SET IDENTITY_INSERT [dbo].[Classes] ON
INSERT INTO [dbo].[Classes] ([Id], [Capacity], [Date], [Name], [Price]) VALUES (1, 4, N'1971-01-01 00:00:00', N'Class A', CAST(10.00 AS Decimal(9, 2)))
INSERT INTO [dbo].[Classes] ([Id], [Capacity], [Date], [Name], [Price]) VALUES (2, 8, N'1982-02-02 00:00:00', N'Class B', CAST(20.00 AS Decimal(9, 2)))
SET IDENTITY_INSERT [dbo].[Classes] OFF

SET IDENTITY_INSERT [dbo].[TypeItems] ON
INSERT INTO [dbo].[TypeItems] ([Id], [Name], [ClassId]) VALUES (3, N'Type A', 1)
INSERT INTO [dbo].[TypeItems] ([Id], [Name], [ClassId]) VALUES (4, N'Type B', 2)
SET IDENTITY_INSERT [dbo].[TypeItems] OFF

SET IDENTITY_INSERT [dbo].[Items] ON
INSERT INTO [dbo].[Items] ([Id], [Description], [Name], [PurchasePrice], [QuantityAvailableForPurchase], [QuantityForRestock], [TypeItemId], [BrandId], [RestockPrice]) VALUES (1, N'AAAAA', N'AaAaA', CAST(15.00 AS Decimal(10, 2)), 7, 40, 3, 1, CAST(12.00 AS Decimal(10, 2)))
INSERT INTO [dbo].[Items] ([Id], [Description], [Name], [PurchasePrice], [QuantityAvailableForPurchase], [QuantityForRestock], [TypeItemId], [BrandId], [RestockPrice]) VALUES (2, N'BBBBB', N'bBbBb', CAST(25.00 AS Decimal(10, 2)), 3, 12, 4, 2, CAST(15.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[Items] OFF

SET IDENTITY_INSERT [dbo].[ItemsForExercise] ON
INSERT INTO [dbo].[ItemsForExercise] ([Id], [Location], [ItemId]) VALUES (1, N'Albacete', 1)
INSERT INTO [dbo].[ItemsForExercise] ([Id], [Location], [ItemId]) VALUES (2, N'Maine', 2)
SET IDENTITY_INSERT [dbo].[ItemsForExercise] OFF
