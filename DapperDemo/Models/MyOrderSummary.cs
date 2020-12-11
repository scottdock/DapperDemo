using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperDemo.Models
{
    public class MyOrderSummary
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public int StaffID { get; set; }

    }
}
