using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);

        IEnumerable<Product> SearchAndFilter(string searchTerm, string searchField, int? categoryId,
   double? minPrice, double? maxPrice, string sortBy, bool sortDescending);

        // 获取低库存产品
        IEnumerable<Product> GetLowStockProducts();

        // 更新销售后的库存
        void UpdateStock(int productId, int quantitySold, string userId);

        // 获取畅销产品
        IEnumerable<Product> GetBestSellingProducts(int count = 10);

    }
}
