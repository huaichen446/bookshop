﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductStockViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<StockHistory> StockHistories { get; set; }

    }
}
