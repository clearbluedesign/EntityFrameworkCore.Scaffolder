﻿using System;
using System.Collections.Generic;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data
{
    public partial class SummaryOfSalesByYear
    {
        public DateTime? ShippedDate { get; set; }
        public int OrderId { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
