using System;
using System.Collections.Generic;
using System.Text;

namespace DapperDemo.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public int StoreID { get; set; }
        public int StaffID { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
