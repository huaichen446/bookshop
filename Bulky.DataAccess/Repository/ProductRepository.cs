using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                objFromDb.ProductImages = obj.ProductImages;
                objFromDb.StockQuantity = obj.StockQuantity;
                objFromDb.LastRestockDate = obj.LastRestockDate;
                objFromDb.LowStockThreshold=obj.LowStockThreshold;
                objFromDb.ViewCount = obj.ViewCount;
                //objFromDb.SalesCount = obj.SalesCount;
                //objFromDb.CreatedDate= obj.CreatedDate;

                //if (obj.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}
            }
        }

        public IEnumerable<Product> SearchAndFilter(string searchTerm, string searchField, int? categoryId,double? minPrice, double? maxPrice, string sortBy, bool sortDescending)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsQueryable();

            // Apply search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();

                if (string.IsNullOrEmpty(searchField) || searchField == "All")
                {
                    // Search in all fields
                    query = query.Where(p =>
                        p.Title.ToLower().Contains(searchTerm) ||
                        p.Author.ToLower().Contains(searchTerm) ||
                        p.ISBN.ToLower().Contains(searchTerm) ||
                        p.Description.ToLower().Contains(searchTerm));
                }
                else
                {
                    // Search in specific field
                    switch (searchField)
                    {
                        case "Title":
                            query = query.Where(p => p.Title.ToLower().Contains(searchTerm));
                            break;
                        case "Author":
                            query = query.Where(p => p.Author.ToLower().Contains(searchTerm));
                            break;
                        case "ISBN":
                            query = query.Where(p => p.ISBN.ToLower().Contains(searchTerm));
                            break;
                    }
                }
            }

            // Apply category filter
            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Apply price range filter
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "Title":
                        query = sortDescending
                            ? query.OrderByDescending(p => p.Title)
                            : query.OrderBy(p => p.Title);
                        break;
                    case "Price":
                        query = sortDescending
                            ? query.OrderByDescending(p => p.Price)
                            : query.OrderBy(p => p.Price);
                        break;
                    case "Author":
                        query = sortDescending
                            ? query.OrderByDescending(p => p.Author)
                            : query.OrderBy(p => p.Author);
                        break;
                    default:
                        query = query.OrderBy(p => p.Title);
                        break;
                }
            }
            else
            {
                // Default sorting
                query = query.OrderBy(p => p.Title);
            }

            return query.ToList();
        }

        public IEnumerable<Product> GetLowStockProducts()
        {
            return _db.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .Include(p => p.Category);
        }

        public IEnumerable<Product> GetBestSellingProducts(int count = 10)
        {
            return _db.Products
                .OrderByDescending(p => p.TotalSaleCount)
                .Take(count)
                .Include(p => p.Category);
        }

        public void UpdateStock(int productId, int quantitySold, string userId)
        {
            var product = _db.Products.Find(productId);
            if (product != null)
            {
                product.StockQuantity -= quantitySold;
                product.TotalSaleCount += quantitySold;
                _db.Products.Update(product);

                // 添加库存历史记录
                var stockHistory = new StockHistory
                {
                    ProductId = productId,
                    QuantityChanged = -quantitySold,
                    Action = "Sale",
                    ApplicationUserId = userId,
                    Notes = "Order Checkout"
                };
                _db.StockHistories.Add(stockHistory);
            }
        }

    }
}
