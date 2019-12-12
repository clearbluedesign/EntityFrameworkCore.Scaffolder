using System;
using System.Collections.Generic;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Models
{
    public partial class OrderSubtotal
    {
        public int OrderId { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
