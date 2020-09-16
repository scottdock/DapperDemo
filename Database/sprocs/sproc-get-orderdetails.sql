USE [BikeStores]
GO

/****** Object:  StoredProcedure [sales].[Get_Order_Details]    Script Date: 9/15/2020 5:54:42 PM ******/
DROP PROCEDURE [sales].[Get_Order_Details]
GO

/****** Object:  StoredProcedure [sales].[Get_Order_Details]    Script Date: 9/15/2020 5:54:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [sales].[Get_Order_Details]
	@OrderID int
AS
BEGIN 
	
	SELECT * FROM sales.orders where orderid = @OrderID; 
    SELECT * from sales.orderitems where orderID = @OrderID;
	RETURN @@ERROR

END
GO


