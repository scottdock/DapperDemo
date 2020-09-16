using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Linq;
using DapperDemo.Models;
using DapperDemo.Extensions;

namespace DapperDemo
{
    class Program
    {
        private static string _connStr = "Server=.;Database=BikeStores;Trusted_Connection=True;";

        static void Main(string[] args)
        {
            //ListDataRaw();
            //ListDataDapperText();
            //ListDataDapperSproc();
            //GetOrderText(1);
            //GetOrderWithDapperSproc(1);
            //GetOrderWithPartialFields(1);
            //GetOrderAndLineItems(1);

            GetWithCustomQueryClass(1);

            var brand = new Models.Brand()
            {
                BrandID = 1,
                BrandName = "U2"
            };

            //UpdateBrandNameWithText(brand);
            //UpdateBrandNameWithTextAmdTrx(brand);
            //UpdateBrandNameWithTextAmdTrx
            //UpdateBrandWithSprocReturnValTransaction(brand);
            //InsertMultipleBrands();

            //ListDataButError();
        }

        private static void GetWithCustomQueryClass(int orderID)
        {
            var query = new Queries.GetOrder(_connStr);
            var request = new Queries.GetOrder.Request()
            {
                OrderID = orderID
            };
            var result = query.Execute(request);
            if (result.Success && result.HasValue)
            {
                Logit($"Found OrderID {result.Order.OrderID}");
            }
            else
            {
                Logit("No data found");
            }
        }


        #region write model

        private static void InsertMultipleBrands()
        {
            var count = -1;

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                db.Open();
                count = db.Execute(@"insert Production.Brands (brandname) values (@a)",
                    new[] { new { a = "Iron Maiden" }, new { a = "Anthrax" }, new { a = "Megadeth" } }
                );

                if (count > 0)
                {
                    Logit($"Brands inserted with result: {count}");
                }
                else
                {
                    Logit($"Error occurred");
                }

                db.Close();
            }

        }

        private static void UpdateBrandWithSprocReturnValTransaction(Models.Brand brand)
        {
            var myRetVal = -1;

            var query = "production.Update_Brands";

            var qp = new DynamicParameters();
            qp.Add(name: "@BrandID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: brand.BrandID);
            qp.Add(name: "@BrandName", dbType: DbType.String, direction: ParameterDirection.Input, value: brand.BrandName);
            qp.Add(name: "@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                db.Open();
                var trx = db.BeginTransaction();
                var result = db.Execute(query, qp, trx, 90, CommandType.StoredProcedure);

                myRetVal = qp.Get<int>("@RetVal");

                Logit($"Result (num records): {result} myRetVal: {myRetVal}");

                if (myRetVal == 0)
                {
                    trx.Commit();
                }
                else
                {
                    trx.Rollback();
                }
                db.Close();
            }

            Logit($"Brand updated with result: {myRetVal}");
            LogitWithPause("Complete");

        }

        private static void UpdateBrandNameWithText(Brand brand)
        {
            var result = -1;

            var query = "update production.brands set brandname = @BrandName where brandid = @BrandID";

            var qp = new DynamicParameters();
            qp.Add(name: "@BrandName", dbType: DbType.String, direction: ParameterDirection.Input, value: brand.BrandName);
            qp.Add(name: "@BrandID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: brand.BrandID);

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                result = db.Execute(query, qp, null, null, CommandType.Text);
            }

            Logit($"Brand updated with result: {result}");
            LogitWithPause("Complete");
        }

        private static void UpdateBrandNameWithTextAmdTrx(Brand brand)
        {
            var result = -1;

            var query = "update production.brands set brandname = @BrandName where brandid = @BrandID";

            var qp = new DynamicParameters();
            qp.Add(name: "@BrandName", dbType: DbType.String, direction: ParameterDirection.Input, value: brand.BrandName);
            qp.Add(name: "@BrandID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: brand.BrandID);

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                db.Open();
                var trx = db.BeginTransaction();
                result = db.Execute(query, qp, trx, null, CommandType.Text);
                if (result > 0)
                {
                    trx.Rollback();
                }
                else
                {
                    trx.Rollback();
                }
                db.Close();
            }

            Logit($"Brand updated with result: {result}");
            LogitWithPause("Complete");
        }

        #endregion

        #region read model

        private static void GetOrderAndLineItems(int orderID)
        {
            Models.Order order = null;

            var query = @"SELECT * FROM sales.orders where orderid = @OrderID; 
                        SELECT * from sales.orderitems where orderID = @OrderID";

            var qp = new DynamicParameters();
            qp.Add("@OrderID", orderID, DbType.Int32, ParameterDirection.Input);

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                var multi = db.QueryMultiple(
                        sql: query,
                        param: qp,
                        commandTimeout: 90,
                        commandType: System.Data.CommandType.Text);

                order = multi.Read<Models.Order>().Single();
                order.OrderItems = multi.Read<Models.OrderItem>().ToList();
            }

            //Logit($"Order {order.OrderID} has {order.OrderItems.Count} line items with total cost of {order.TotalPurchaseAmount}");

            var crossCheckOrderAmount = order.OrderItems.Sum(x => x.LineItemPrice);

            Logit($"Cross Calculation {crossCheckOrderAmount}");

        }

        private static void GetOrderWithPartialFields(int orderid)
        {
            var result = new Models.OrderSummary();

            var query = "sales.Get_Order";

            var qp = new DynamicParameters();
            qp.Add(name: "@OrderID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: orderid);

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                result = db.QueryFirstOrDefault<Models.OrderSummary>(
                        sql: query,
                        commandTimeout: 90,
                        commandType: System.Data.CommandType.StoredProcedure,
                        param: qp
                        );
            }

            Logit($"Order {result.OrderID} has a current status of {result.OrderStatus}");
            LogitWithPause("Complete");

        }

        private static void ListDataRaw()
        {
            var myResult = new List<Models.Order>();

            var query = "SELECT top 10 * FROM sales.orders";

            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();

                var cmd = new SqlCommand(query, conn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 90;

                var reader = cmd.ExecuteReader();

                using (var dt = new DataTable())
                {
                    dt.Load(reader);
                    var myCount = dt.Rows.Count;

                    if (myCount > 0)
                    {
                        foreach (DataRow myRow in dt.Rows)
                        {
                            var obj = new Models.Order()
                            {
                                OrderID = (int)myRow["orderid"],
                                CustomerID = (int)myRow["customerid"],
                                OrderStatus = (byte)myRow["orderstatus"],
                                OrderDate = (DateTime)myRow["orderdate"],
                                RequiredDate = (DateTime)myRow["RequiredDate"],
                                ShippedDate = (DateTime)myRow["ShippedDate"],
                                StoreID = (int)myRow["StoreID"],
                                StaffID = (int)myRow["StaffID"]
                            };

                            myResult.Add(obj);
                        }
                    }
                }

                conn.Close();
            }

            Logit("Records from Raw query:");

            foreach (var item in myResult)
            {
                Logit($"Order {item.OrderID} was placed at {item.OrderDate} and shipped on {item.ShippedDate}");
            }

            LogitWithPause("Complete");

        }

        private static void ListDataDapperText()
        {
            var myResult = new List<Models.Order>();

            var query = "SELECT top 10 * FROM sales.orders";

            using (var conn = new SqlConnection(_connStr))
            {
                myResult = conn.Query<Models.Order>(
                    sql: query,
                    commandTimeout: 90,
                    commandType: CommandType.Text).ToList();
            }

            Logit("Records from Raw query:");

            foreach (var item in myResult)
            {
                Logit($"Order {item.OrderID} was placed at {item.OrderDate} and shipped on {item.ShippedDate}");
            }

            LogitWithPause("Complete");

        }

        private static void GetOrderText(int orderID)
        {
            Models.Order myResult;

            var query = "SELECT * from sales.orders where orderid = @OrderID";

            var qp = new DynamicParameters();
            qp.Add(name: "@OrderID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: orderID);

            using (var conn = new SqlConnection(_connStr))
            {
                myResult = conn.QueryFirstOrDefault<Models.Order>(
                    sql: query,
                    param: qp,
                    commandTimeout: 90,
                    commandType: CommandType.Text);
            }

            Logit("Records from Raw query:");

            Logit($"Order {myResult.OrderID} was placed at {myResult.OrderDate} and shipped on {myResult.ShippedDate}");

            LogitWithPause("Complete");

        }

        private static void GetOrderWithDapperSproc(int orderid)
        {
            var result = new Models.Order();
            var query = "sales.Get_Order";

            var qp = new DynamicParameters();
            qp.Add(name: "@OrderID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: orderid);

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                result = db.QueryFirstOrDefault<Models.Order>(
                        sql: query,
                        commandTimeout: 90,
                        commandType: System.Data.CommandType.StoredProcedure,
                        param: qp
                        );
            }

            Logit($"Order {result.OrderID} was placed at {result.OrderDate} and shipped on {result.ShippedDate}");
            LogitWithPause("Complete");
        }

        private static void ListDataDapperSproc()
        {
            var myResult = new List<Models.Order>();

            var query = "sales.Get_top10_orders";

            using (var conn = new SqlConnection(_connStr))
            {
                myResult = conn.Query<Models.Order>(
                    sql: query,
                    commandTimeout: 90,
                    commandType: CommandType.StoredProcedure).ToList();
            }

            Logit("Records from Raw query:");

            foreach (var item in myResult)
            {
                Logit($"Order {item.OrderID} was placed at {item.OrderDate} and shipped on {item.ShippedDate}");
            }

            LogitWithPause("Complete");

        }

        #endregion

        #region misc
        private static void ListDataButError()
        {
            var results = new List<Models.Order>();

            try
            {
                using (System.Data.IDbConnection db = new SqlConnection(_connStr))
                {
                    results = db.Query<Models.Order>(
                            sql: "select 1/0, * from sales.Orders",
                            commandTimeout: 90,
                            commandType: System.Data.CommandType.Text).ToList();
                }

                Logit("Records from Raw query:");

                foreach (var item in results)
                {
                    Logit($"Order {item.OrderID} was placed at {item.OrderDate} and shipped on {item.ShippedDate}");
                }

                LogitWithPause("Complete");
            }
            catch (SqlException sqlex)
            {
                Logit(sqlex.GetFullMessage());
            }
            catch (Exception ex)
            {
                Logit(ex.GetFullMessage());
            }

        }

        #endregion

        #region supporting methods 
        private static void LogitWithPause(string v)
        {
            Console.WriteLine(v);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        private static void Logit(string v)
        {
            Console.WriteLine(v);
        }
        #endregion

    }
}
