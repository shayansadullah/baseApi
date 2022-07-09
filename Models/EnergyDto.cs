using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensek.Models
{
    public class Electric
    {
        public int energy_id { get; set; }
        public double price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    }

    public class Gas
    {
        public int energy_id { get; set; }
        public double price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    }

    public class Nuclear
    {
        public int energy_id { get; set; }
        public double price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    }

    public class Oil
    {
        public int energy_id { get; set; }
        public double price_per_unit { get; set; }
        public int quantity_of_units { get; set; }
        public string unit_type { get; set; }
    }

    public class EnergyDto
    {
        public Electric electric { get; set; }
        public Gas gas { get; set; }
        public Nuclear nuclear { get; set; }
        public Oil oil { get; set; }
    }
}
