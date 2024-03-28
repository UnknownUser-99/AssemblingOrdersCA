using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblingOrdersCA
{
    public class ProductShelf
    {
        public int ProductID { get; set; }
        public int ShelfID { get; set; }
        public bool IsMainShelf { get; set; }
        public int ProductShelfQuantity { get; set; }
        public virtual Product Product { get; set; }
        public virtual Shelf Shelf { get; set; }
    }
}
