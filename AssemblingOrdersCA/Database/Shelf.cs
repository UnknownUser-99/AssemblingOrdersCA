using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblingOrdersCA
{
    public class Shelf
    {
        public int ShelfID { get; set; }
        public string ShelfName { get; set; }
        public virtual ICollection<ProductShelf> ProductShelf { get; set; }
    }
}
