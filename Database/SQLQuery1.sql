--select * from sales.orders where orderid = 1
--select * from sales.orderitems where orderid = 1


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

