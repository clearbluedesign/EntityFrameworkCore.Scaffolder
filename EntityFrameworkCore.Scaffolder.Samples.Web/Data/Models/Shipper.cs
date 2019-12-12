using System;
using System.Collections.Generic;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Models
{
    public partial class Shipper
    {
        public int ShipperId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
