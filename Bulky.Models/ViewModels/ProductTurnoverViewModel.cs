using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductTurnoverViewModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int CurrentStock { get; set; }
        public int TotalSales { get; set; }
        public double TurnoverRate { get; set; }

    }
}
