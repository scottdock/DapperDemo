using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using DapperDemo.Extensions;

namespace DapperDemo.Queries
{
    public class GetOrder
    {
        public static string _connStr = string.Empty;

        public GetOrder(string connStr)
        {
            _connStr = connStr;
        }

        public class Request
        {
            public int OrderID { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public bool HasValue { get; set; }
            public string ErrorMessage { get; set; }
            public Models.Order Order { get; set; }
        }

        public Response Execute(Request request)
        {
            var response = new Response()
            {
                ErrorMessage = string.Empty,
                HasValue = false,
                Order = null,
                Success = false
            };

            var query = "sales.Get_Order";

            var qp = new DynamicParameters();
            qp.Add(name: "@OrderID", dbType: DbType.Int32, direction: ParameterDirection.Input, value: request.OrderID);

            try
            {
                using (System.Data.IDbConnection db = new SqlConnection(_connStr))
                {
                    var result = db.QueryFirstOrDefault<Models.Order>(
                            sql: query,
                            commandTimeout: 90,
                            commandType: System.Data.CommandType.StoredProcedure,
                            param: qp
                            );

                    response.Success = true;
                    response.HasValue = (result != null && result.OrderID > 0);
                    response.Order = result;
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

    }
	
}
