USE [BikeStores]
GO

/****** Object:  StoredProcedure [sales].[Get_Order]    Script Date: 9/15/2020 5:57:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER procedure [sales].[Get_Order]
	@orderid int
AS
BEGIN

	SELECT 
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
		orderid = @orderid 

END 
GO


