/*
--------------------------------------------------------------------
© 2017 sqlservertutorial.net All Rights Reserved
--------------------------------------------------------------------
Name   : BikeStores
Link   : http://www.sqlservertutorial.net/load-sample-database/
Version: 1.0
--------------------------------------------------------------------
*/
-- create schemas
CREATE SCHEMA production;
go

CREATE SCHEMA sales;
go

-- create tables
CREATE TABLE production.categories (
	categoryid INT IDENTITY (1, 1) PRIMARY KEY,
	categoryname VARCHAR (255) NOT NULL
);

CREATE TABLE production.brands (
	brandid INT IDENTITY (1, 1) PRIMARY KEY,
	brandname VARCHAR (255) NOT NULL
);

CREATE TABLE production.products (
	productid INT IDENTITY (1, 1) PRIMARY KEY,
	productname VARCHAR (255) NOT NULL,
	brandid INT NOT NULL,
	categoryid INT NOT NULL,
	modelyear SMALLINT NOT NULL,
	listprice DECIMAL (10, 2) NOT NULL,
	FOREIGN KEY (categoryid) REFERENCES production.categories (categoryid) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (brandid) REFERENCES production.brands (brandid) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE sales.customers (
	customerid INT IDENTITY (1, 1) PRIMARY KEY,
	firstname VARCHAR (255) NOT NULL,
	lastname VARCHAR (255) NOT NULL,
	phone VARCHAR (25),
	email VARCHAR (255) NOT NULL,
	street VARCHAR (255),
	city VARCHAR (50),
	state VARCHAR (25),
	zipcode VARCHAR (5)
);

CREATE TABLE sales.stores (
	storeid INT IDENTITY (1, 1) PRIMARY KEY,
	storename VARCHAR (255) NOT NULL,
	phone VARCHAR (25),
	email VARCHAR (255),
	street VARCHAR (255),
	city VARCHAR (255),
	state VARCHAR (10),
	zipcode VARCHAR (5)
);

CREATE TABLE sales.staffs (
	staffid INT IDENTITY (1, 1) PRIMARY KEY,
	firstname VARCHAR (50) NOT NULL,
	lastname VARCHAR (50) NOT NULL,
	email VARCHAR (255) NOT NULL UNIQUE,
	phone VARCHAR (25),
	active tinyint NOT NULL,
	storeid INT NOT NULL,
	managerid INT,
	FOREIGN KEY (storeid) REFERENCES sales.stores (storeid) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (managerid) REFERENCES sales.staffs (staffid) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE sales.orders (
	orderid INT IDENTITY (1, 1) PRIMARY KEY,
	customerid INT,
	orderstatus tinyint NOT NULL,
	-- Order status: 1 = Pending; 2 = Processing; 3 = Rejected; 4 = Completed
	orderdate DATE NOT NULL,
	requireddate DATE NOT NULL,
	shippeddate DATE,
	storeid INT NOT NULL,
	staffid INT NOT NULL,
	FOREIGN KEY (customerid) REFERENCES sales.customers (customerid) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (storeid) REFERENCES sales.stores (storeid) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (staffid) REFERENCES sales.staffs (staffid) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE sales.orderitems (
	orderid INT,
	itemid INT,
	productid INT NOT NULL,
	quantity INT NOT NULL,
	listprice DECIMAL (10, 2) NOT NULL,
	discount DECIMAL (4, 2) NOT NULL DEFAULT 0,
	PRIMARY KEY (orderid, itemid),
	FOREIGN KEY (orderid) REFERENCES sales.orders (orderid) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (productid) REFERENCES production.products (productid) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE production.stocks (
	storeid INT,
	productid INT,
	quantity INT,
	PRIMARY KEY (storeid, productid),
	FOREIGN KEY (storeid) REFERENCES sales.stores (storeid) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (productid) REFERENCES production.products (productid) ON DELETE CASCADE ON UPDATE CASCADE
);