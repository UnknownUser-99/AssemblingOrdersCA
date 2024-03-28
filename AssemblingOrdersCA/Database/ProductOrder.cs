using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblingOrdersCA
{
    public class ProductOrder
    {
        public int ProductID { get; set; }
        public int OrderID { get; set; }
        public int ProductOrderQuantity { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}
