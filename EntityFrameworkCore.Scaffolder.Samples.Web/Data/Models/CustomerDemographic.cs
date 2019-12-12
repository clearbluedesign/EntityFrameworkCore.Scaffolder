using System;
using System.Collections.Generic;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data
{
    public partial class CustomerDemographic
    {
        public string CustomerTypeId { get; set; }
        public string CustomerDesc { get; set; }
        public virtual ICollection<CustomerCustomerDemo> CustomerCustomerDemoes { get; set; } = new HashSet<CustomerCustomerDemo>();
    }
}
