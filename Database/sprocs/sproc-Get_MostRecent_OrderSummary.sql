USE [BikeStores]
GO

DROP PROCEDURE [sales].[Get_MostRecent_OrderSummary]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sales].[Get_MostRecent_OrderSummary]
AS
BEGIN

select top 5 
	o.OrderID
	, o.CustomerID
	, c.FirstName + ' ' + c.LastName as CustomerFullName
	, o.OrderStatus
	, o.OrderDate
	, o.RequiredDate
	, o.ShippedDate
	, o.StoreID
	, s.StoreName
	, o.StaffID
	, c.FirstName + ' ' + c.LastName as SalesAssociateFullName
from 
	sales.orders o
	join sales.customers c on c.CustomerID = o.CustomerID
	join sales.stores s on s.StoreID = o.StoreID 
	join sales.Staffs st on st.StaffID = o.StaffID
--where 
--	o.OrderID = 1
order by 
	o.OrderDate desc 


END 
GO


