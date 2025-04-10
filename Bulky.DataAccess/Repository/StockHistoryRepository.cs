using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class StockHistoryRepository : Repository<StockHistory>, IStockHistoryRepository
    {
        private ApplicationDbContext _db;

        public StockHistoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<StockHistory> GetProductHistory(int productId)
        {
            return _db.StockHistories
                .Where(s => s.ProductId == productId)
                .Include(s => s.ApplicationUser)
                .OrderByDescending(s => s.Date);
        }

        public void Update(StockHistory stockHistory)
        {
            _db.StockHistories.Update(stockHistory);
        }

        public void Add(StockHistory stockHistory) 
        {

        }

    }
}
