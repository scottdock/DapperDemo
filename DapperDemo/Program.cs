using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Data;
using DapperDemo.Models;

namespace DapperDemo
{
    class Program
    {
        private static string _connStr = "Server=.;Database=BikeStores;Trusted_Connection=True;";


        static void Main(string[] args)
        {
            // ===> Queries 
            // Get multiple records via ADO.NET
            //LoadOrdersRaw();

            // Get multiple via Dapper - text 
            //LoadOrdersDapperText();
            //LoadOrdersDapperTextSummary();

            // Get multiple via Dapper - stored procedure
            //LoadOrdersDapperSproc();

            // Get Single via sproc 
            //GetOrderText(1);
            // Get Single - summary 
            // Get single Parent & Child object collection  
            //GetOrderAndLineItems(1);

            // ===> Commands, Transactions, Return Values 
            // Update with Text 
            //var b = new Models.Brand()
            //{
            //    BrandID = 9,
            //    BrandName = "Hall & Oates"
            //};
            //UpdateBrandNameWithText(b);

            // update with text & transaction 
            //UpdateBrandNameWithTextAndTrx(b);

            // Update with sproc & return values 
            //UpdateBrandWithSprocReturnValTransaction(b);

            // insert with sproc 
            // INsert multiple values with single command 
            //InsertBrandSproc();

            // ===> Exception Handling 
            // Simulate Error from sproc 
            //ListDataButError();

            // ===> Approach 
            // Nice concise query approach 
            GetOrderCustom(1);
        }

        private static void GetOrderCustom(int v)
        {
            var query = new Queries.GetOrder(_connStr);
            var request = new Queries.GetOrder.Request()
            {
                OrderID = 1
            };
            var result = query.Execute(request);

            if (result.Success && result.HasValue)
            {
                //
            }
        }

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
                Logit(sqlex.Message);
            }
            catch (Exception ex)
            {
                Logit(ex.Message);
            }

        }
        private static void InsertBrandSproc()
        {
            var count = -1;

            using (System.Data.IDbConnection db = new SqlConnection(_connStr))
            {
                count = db.Execute(@"insert Production.Brands (brandname) values (@a)",
                    new[] { new { a = "Miles Davis" }, new { a = "Dave Weckl" }, new { a = "Chick Corea" } }
                );
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

                //Logit($"Result (num records): {result} myRetVal: {myRetVal}");

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
		
		
        private static void UpdateBrandNameWithTextAndTrx(Brand brand)
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
                    trx.Commit();
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

            var totalOrderAmount = order.OrderItems.Sum(x => x.LineItemPrice);
            Logit($"Order {order.OrderID} has {order.OrderItems.Count} line items at a total cost of {totalOrderAmount}");

        }
        private static void GetOrderText(int orderID)
        {
            var myResult = new Models.Order();

            var query = "SELECT * FROM sales.orders where orderid = @OrderID";

            var qp = new DynamicParameters();
            qp.Add(name: "@OrderID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: orderID);

            using (var conn = new SqlConnection(_connStr))
            {
                myResult = conn.QueryFirstOrDefault<Models.Order>(
                    sql: query,
                    param: qp,
                    commandTimeout: 90,
                    commandType: System.Data.CommandType.Text);
            }

            Logit("Records from Raw query:");

            Logit($"Order {myResult.OrderID} was placed at {myResult.OrderDate} and shipped on {myResult.ShippedDate}");

            LogitWithPause("Complete");

        }



        private static void LoadOrdersDapperSproc()
        {
            var myResult = new List<Models.Order>();

            var query = "[sales].[Get_MostRecent_Orders]";

            using (var conn = new SqlConnection(_connStr))
            {
                myResult = conn.Query<Models.Order>(
                    sql: query,
                    commandTimeout: 90,
                    commandType: System.Data.CommandType.StoredProcedure).ToList();
            }

            Logit("Records from Raw query:");

            foreach (var item in myResult)
            {
                Logit($"Order {item.OrderID} was placed at {item.OrderDate} and shipped on {item.ShippedDate}");
            }

            LogitWithPause("Complete");

        }


        private static void LoadOrdersDapperText()
        {
            var myResult = new List<Models.Order>();

            var query = "SELECT top 10 * FROM sales.orders";

            using (var conn = new SqlConnection(_connStr))
            {
                myResult = conn.Query<Models.Order>(
                    sql: query,
                    commandTimeout: 90,
                    commandType: System.Data.CommandType.Text).ToList();
            }

            Logit("Records from Raw query:");

            foreach (var item in myResult)
            {
                Logit($"Order {item.OrderID} was placed at {item.OrderDate} and shipped on {item.ShippedDate}");
            }

            LogitWithPause("Complete");

        }

        private static void LoadOrdersDapperTextSummary()
        {
            var myResult = new List<Models.MyOrderSummary>();

            var query = "SELECT top 10 * FROM sales.orders";

            using (var conn = new SqlConnection(_connStr))
            {
                myResult = conn.Query<Models.MyOrderSummary>(
                    sql: query,
                    commandTimeout: 90,
                    commandType: System.Data.CommandType.Text).ToList();
            }



            Logit("Records from Raw query:");

            foreach (var item in myResult)
            {
                Logit($"Order {item.OrderID} was placed at {item.OrderDate} from sales person {item.StaffID}");
            }

            LogitWithPause("Complete");

        }

        private static void LoadOrdersRaw()
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

                while (reader.Read())
                {
                    var obj = new Models.Order()
                    {
                        OrderID = (int)reader["orderid"],
                        CustomerID = (int)reader["customerid"],
                        OrderStatus = (byte)reader["orderstatus"],
                        OrderDate = (DateTime)reader["orderdate"],
                        RequiredDate = (DateTime)reader["RequiredDate"],
                        ShippedDate = (DateTime)reader["ShippedDate"],
                        StoreID = (int)reader["StoreID"],
                        StaffID = (int)reader["StaffID"]
                    };

                    myResult.Add(obj);
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



        #region supporting methods 
        private static void LogitWithPause(string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        private static void Logit(string msg)
        {
            Console.WriteLine(msg);
        }
        #endregion

    }




}
