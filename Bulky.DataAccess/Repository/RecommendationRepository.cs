using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly ApplicationDbContext _db;

        public RecommendationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        #region 基础推荐

        // 热门产品推荐 - 基于销售量和评分
        public IEnumerable<Product> GetPopularProducts(int count = 2)
        {
            return _db.Products
                .Select(p => new
                {
                    Product = p,
                    AverageRating = _db.Reviews
                .Where(r => r.ProductId == p.Id && r.IsApproved)
                .Select(r => (double?)r.Rating)
                .Average() ?? 0
                })
               .OrderByDescending(p =>
                   p.Product.TotalSaleCount * 0.5 +
                   p.AverageRating * 10)
               .Take(count)
               .Select(p => p.Product)
               .Include(p => p.Category)
               .Include(p => p.ProductImages)
               .ToList();
        }

        // 新书推荐
        //public IEnumerable<Product> GetNewReleases(int count = 1)
        //{
        //    return _db.Products
        //        .OrderByDescending(p => p.CreatedDate)
        //        .Take(count)
        //        .Include(p => p.Category)
        //        .Include(p => p.ProductImages)
        //        .ToList();
        //}

        #endregion

        #region 个性化推荐

        //为用户提供个性化推荐
        public IEnumerable<Product> GetPersonalizedRecommendations(string userId, int count = 2)
        {
            // 获取用户最近浏览/购买的类别
            var userCategoryIds=_db.UserActivities
                .Where(a => a.UserId ==userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(2)
                .Join(_db.Products,
                  a => a.ProductId,
                  p => p.Id,
                  (a, p) => p.CategoryId)
                .Distinct()
                .ToList();

            // 获取用户最近浏览/购买的作者
            var userAuthors = _db.UserActivities
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(2)
                .Join(_db.Products,
                      a => a.ProductId,
                      p => p.Id,
                      (a, p) => p.Author)
                .Distinct()
                .ToList();

            // 用户最近查看过的产品
            var recentViewedProductIds = _db.UserActivities
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(2)
                .Select(a => a.ProductId)
                .ToList();

            // 根据用户行为协同过滤
            var recommendedProducts = new List<Product>();

            // 添加基于类别的推荐
            foreach (var categoryId in userCategoryIds)
            {
                var categoryProducts = _db.Products
                    .Where(p => p.CategoryId == categoryId &&
                           !recentViewedProductIds.Contains(p.Id))
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .OrderByDescending(p => p.TotalSaleCount)
                    .Take(2)
                    .ToList();

                recommendedProducts.AddRange(categoryProducts);
            }

            // 添加基于作者的推荐
            foreach (var author in userAuthors)
            {
                //var authorProducts = _db.Products
                //    .Where(p =>  p.Author == author &&
                //           !recentViewedProductIds.Contains(p.Id) &&
                //           !recommendedProducts.Any(rp => rp.Id == p.Id))
                //    .Include(p => p.Category)
                //    .Include(p => p.ProductImages)
                //    .OrderByDescending(p => p.TotalSaleCount)
                //    .Take(2)
                //    .ToList();
                var authorProducts = _db.Products
                      .Where(p => p.Author == author &&
                   !recentViewedProductIds.Contains(p.Id))
                      .AsEnumerable() // 切换为客户端处理
                      .Where(p => !recommendedProducts.Any(rp => rp.Id == p.Id))
                      .OrderByDescending(p => p.TotalSaleCount)
                      .Take(2)
                      .ToList();

                recommendedProducts.AddRange(authorProducts);
            }

            // 通过预先计算的产品关系添加推荐
            foreach (var productId in recentViewedProductIds)
            {
                //var relatedProducts = _db.ProductRelations
                //    .Where(r => r.ProductId1 == productId)
                //    .OrderByDescending(r => r.RelationScore)
                //    .Take(2)
                //    .Join(_db.Products,
                //          r => r.ProductId2,
                //          p => p.Id,
                //          (r, p) => new { Product = p, Score = r.RelationScore })
                //    .Where(x => ! recentViewedProductIds.Contains(x.Product.Id) &&
                //           !recommendedProducts.Any(rp => rp.Id == x.Product.Id))
                //    .Select(x => x.Product)
                //    .Include(p => p.Category)
                //    .Include(p => p.ProductImages)
                //    .Take(3)
                //    .ToList();
                var relatedProducts = _db.ProductRelations
                      .Where(r => r.ProductId1 == productId)
                      .OrderByDescending(r => r.RelationScore)
                      .Take(2)
                      .Join(_db.Products,
                           r => r.ProductId2,
                           p => p.Id,
                           (r, p) => new { Product = p, Score = r.RelationScore })
                      .AsEnumerable() // 切换到客户端执行以下逻辑
                      .Where(x => !recentViewedProductIds.Contains(x.Product.Id) &&
                           !recommendedProducts.Any(rp => rp.Id == x.Product.Id))
                      .Select(x => x.Product)
                      .Take(2) // 注意：现在在内存中排序和截取
                      .ToList();

                recommendedProducts.AddRange(relatedProducts);
            }

            // 如果推荐不足，添加一些常规热门产品
            if (recommendedProducts.Count < count)
            {
                //var additionalProducts = GetPopularProducts(count)
                //    .Where(p => !recentViewedProductIds.Contains(p.Id) &&
                //           !recommendedProducts.Any(rp => rp.Id == p.Id))
                //    .Take(count - recommendedProducts.Count)
                //    .ToList();

                var additionalProducts = GetPopularProducts(count)
                      .AsEnumerable() // 切换到客户端处理后续的 Where 和 Any 逻辑
                      .Where(p => !recentViewedProductIds.Contains(p.Id) &&
                             !recommendedProducts.Any(rp => rp.Id == p.Id))
                      .Take(count - recommendedProducts.Count)
                      .ToList();

                recommendedProducts.AddRange(additionalProducts);
            }

            // 打乱顺序，返回指定数量
            return recommendedProducts
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToList();

        }

        #endregion

        #region 基于内容的推荐

        // 相关产品推荐 - 使用预计算的相关度数据
        public IEnumerable<Product> GetRelatedProducts(int productId, int count = 2)
        {
            // 首先尝试使用预计算的相关性分数
            var relatedProducts = _db.ProductRelations
                .Where(r => r.ProductId1 == productId)
                .OrderByDescending(r => r.RelationScore)
                .Take(count)
                .Join(_db.Products,
                      r => r.ProductId2,
                      p => p.Id,
                      (r, p) => p)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToList();

            // 如果没有足够的预计算相关产品，则回退到基于类别和作者的简单推荐
            if (relatedProducts.Count < count)
            {
                var product = _db.Products.Find(productId);
                if (product == null) return relatedProducts;

                var fallbackProducts = _db.Products
                    .Where(p =>  p.Id != productId &&
                           (p.CategoryId == product.CategoryId || p.Author == product.Author) &&
                           !relatedProducts.Any(rp => rp.Id == p.Id))
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .OrderByDescending(p => p.TotalSaleCount)
                    .Take(count - relatedProducts.Count)
                    .ToList();

                relatedProducts.AddRange(fallbackProducts);
            }

            return relatedProducts;
        }

        // 基于类别的推荐
        public IEnumerable<Product> GetProductsByCategory(int categoryId, int count = 2)
        {
            return _db.Products
                .Where(p =>p.CategoryId == categoryId)
                .OrderByDescending(p => p.TotalSaleCount)
                .Take(count)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToList();
        }

        #endregion

        #region 用户行为和产品关系管理
        // 记录用户活动
        public void RecordUserActivity(string userId, int productId, ActivityType activityType, int? rating = null)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                ProductId = productId,
                ActivityType = activityType,
                //Rating = rating,
                Timestamp = DateTime.Now
            };

            _db.UserActivities.Add(activity);
            _db.SaveChanges();

            // 如果是购买行为，更新产品销售计数
            //if (activityType == ActivityType.Purchase)
            //{
            //    var product = _db.Products.Find(productId);
            //    if (product != null)
            //    {
            //        product.TotalSaleCount += 1;
            //        _db.Products.Update(product);
            //        _db.SaveChanges();
            //    }
            //}
        }



        //一下代码我没有使用，因为逻辑归过于复杂，而我们的数据量太小
        // 生成产品关联度数据 - 可由定时任务定期执行
        public void GenerateProductRelations()
        {
            // 清空现有关联数据
            _db.ProductRelations.RemoveRange(_db.ProductRelations);
            _db.SaveChanges();

            // 获取所有产品ID
            var productIds = _db.Products.Select(p => p.Id).ToList();

            // 为每对产品生成关联度分数
            var relations = new List<ProductRelation>();

            foreach (var id1 in productIds)
            {
                var product1 = _db.Products.Find(id1);

                foreach (var id2 in productIds.Where(id => id > id1)) // 避免重复计算
                {
                    var product2 = _db.Products.Find(id2);
                    double relationScore = 0;

                    // 类别相同 +0.3
                    if (product1.CategoryId == product2.CategoryId)
                        relationScore += 0.3;

                    // 作者相同 +0.4
                    if (product1.Author == product2.Author)
                        relationScore += 0.4;

                    // 标题相似度 (简单实现)
                    var title1Words = product1.Title.ToLower().Split(' ');
                    var title2Words = product2.Title.ToLower().Split(' ');
                    var commonWords = title1Words.Intersect(title2Words).Count();
                    relationScore += 0.1 * commonWords / Math.Max(title1Words.Length, title2Words.Length);

                    // 共同购买/浏览的用户比例 (协同过滤核心)
                    var users1 = _db.UserActivities
                        .Where(a => a.ProductId == id1)
                        .Select(a => a.UserId)
                        .Distinct()
                        .ToList();

                    var users2 = _db.UserActivities
                        .Where(a => a.ProductId == id2)
                        .Select(a => a.UserId)
                        .Distinct()
                        .ToList();

                    var commonUsers = users1.Intersect(users2).Count();

                    if (users1.Count > 0 && users2.Count > 0)
                    {
                        // 杰卡德系数 - 共同用户数 / 总用户数
                        relationScore += 0.5 * commonUsers / (users1.Count + users2.Count - commonUsers);
                    }

                    // 价格范围相似度
                    var priceDiff = Math.Abs(product1.Price - product2.Price) / Math.Max(product1.Price, product2.Price);
                    relationScore += 0.1 * (1 - priceDiff);

                    // 保存显著的关联 (分数 > 0.2)
                    if (relationScore > 0.2)
                    {
                        relations.Add(new ProductRelation
                        {
                            ProductId1 = id1,
                            ProductId2 = id2,
                            RelationScore = relationScore
                        });

                        // 对称关系
                        relations.Add(new ProductRelation
                        {
                            ProductId1 = id2,
                            ProductId2 = id1,
                            RelationScore = relationScore
                        });
                    }
                }
            }

            // 批量插入关系
            _db.ProductRelations.AddRange(relations);
            _db.SaveChanges();
        }

        #endregion




    }
}
