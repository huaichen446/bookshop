using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.Models;

namespace BulkyBook.Models.ViewModels
{
    public class RecommendationVM
    {
        public IEnumerable<Product> PersonalizedProducts { get; set; }
        public IEnumerable<Product> PopularProducts { get; set; }
        //public IEnumerable<Product> NewProducts { get; set; }

    }
}
