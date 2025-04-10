using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.DataAcess.Data;
using Microsoft.EntityFrameworkCore;


namespace BulkyBook.DataAccess.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private ApplicationDbContext _db;

        public ReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public IEnumerable<Review> GetAllByProduct(int productId, bool includeUnapproved = false)
        //{
        //    if (includeUnapproved)
        //    {
        //        return _db.Reviews.Where(r => r.ProductId == productId)
        //            .Include(r => r.ApplicationUser);
        //    }
        //    else
        //    {
        //        return _db.Reviews.Where(r => r.ProductId == productId && r.IsApproved)
        //            .Include(r => r.ApplicationUser);
        //    }
        //}

        public IEnumerable<Review> GetAllByProduct(int productId, bool includeUnapproved = false)
        {
            IQueryable<Review> query;

            if (includeUnapproved)
            {
                query = _db.Reviews.Where(r => r.ProductId == productId)
                    .Include(r => r.ApplicationUser);
            }
            else
            {
                query = _db.Reviews.Where(r => r.ProductId == productId && r.IsApproved)
                    .Include(r => r.ApplicationUser);
            }

            // 确保返回列表而不是IQueryable
            return query.ToList();
        }

        public double GetAverageRating(int productId)
        {
            var reviews = _db.Reviews.Where(r => r.ProductId == productId && r.IsApproved);
            if (!reviews.Any()) return 0;
            return reviews.Average(r => r.Rating);
        }

        public void Update(Review review)
        {
            _db.Reviews.Update(review);
        }

    }
}
