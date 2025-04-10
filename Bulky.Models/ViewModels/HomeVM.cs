using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        // 添加推荐产品列表
        public IEnumerable<Product> PopularProducts { get; set; }
        public IEnumerable<Product> NewProducts { get; set; }
    }
}
