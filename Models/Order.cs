using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensek.Models
{
    public class Order
    {
        public string fuel { get; set; }
        public string id { get; set; }
        public int quantity { get; set; }
        public DateTime time { get; set; }
    }
}
