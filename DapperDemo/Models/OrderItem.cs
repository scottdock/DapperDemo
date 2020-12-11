using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDemo.Models
{    
    public class OrderItem
    {
        public int OrderID { get; set; }
        public int ItemID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Discount { get; set; }

        public decimal LineItemPrice
        {
            get
            {
                return (this.ListPrice * this.Quantity) * (1 - this.Discount);
            }
        }
    }
}
