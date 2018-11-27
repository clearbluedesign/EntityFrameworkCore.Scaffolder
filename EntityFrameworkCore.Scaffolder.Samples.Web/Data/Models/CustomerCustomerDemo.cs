using System;
using System.Collections.Generic;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Models
{
    public partial class CustomerCustomerDemo
    {
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public string CustomerTypeId { get; set; }
        public virtual CustomerDemographic CustomerType { get; set; }
    }
}
