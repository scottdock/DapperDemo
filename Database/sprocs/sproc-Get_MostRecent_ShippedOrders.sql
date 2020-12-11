USE [BikeStores]
GO

/****** Object:  StoredProcedure [sales].[Get_MostRecent_ShippedOrders]    Script Date: 9/15/2020 5:57:33 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER procedure [sales].[Get_MostRecent_ShippedOrders]
AS
BEGIN

	SELECT --top 10
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
	where
		shippeddate is not null
	order by 
		orderid desc 

END 
GO


