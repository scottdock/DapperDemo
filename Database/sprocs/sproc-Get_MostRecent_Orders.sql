USE [BikeStores2]
GO

/****** Object:  StoredProcedure [sales].[Get_MostRecent_Orders]    Script Date: 9/15/2020 5:57:26 AM ******/
DROP PROCEDURE [sales].[Get_MostRecent_Orders]
GO

/****** Object:  StoredProcedure [sales].[Get_MostRecent_Orders]    Script Date: 9/15/2020 5:57:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sales].[Get_MostRecent_Orders]
AS
BEGIN

	SELECT top 10
		[orderid]
		,[customerid]
		,[orderstatus]
		,[orderdate]
		,[requireddate]
		,[shippeddate]
		,[storeid]
		,[staffid]
	FROM 
		[sales].[orders]
	order by 
		orderid desc 

END 
GO


