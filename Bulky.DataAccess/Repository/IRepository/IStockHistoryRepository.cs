using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IStockHistoryRepository
    {
        void Update(StockHistory stockHistory);

        IEnumerable<StockHistory> GetProductHistory(int productId);

        void Add(StockHistory stockHistory);

    }
}
