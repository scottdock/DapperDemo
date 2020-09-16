USE [BikeStores]
GO

/****** Object:  StoredProcedure [production].[Get_Brands_Eraseme]    Script Date: 9/15/2020 5:54:18 PM ******/
DROP PROCEDURE [production].[Get_Brands_Eraseme]
GO

/****** Object:  StoredProcedure [production].[Get_Brands_Eraseme]    Script Date: 9/15/2020 5:54:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [production].[Get_Brands_Eraseme]
AS

BEGIN

	select * from production.brands

END 
GO


