using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRecommendationRepository
    {
        //IEnumerable<Product> GetPopularProducts(int count = 2);
        //IEnumerable<Product> GetRelatedProducts(int productId, int count = 2);
        //IEnumerable<Product> GetNewProducts(int count = 2);

        // 基础推荐
        IEnumerable<Product> GetPopularProducts(int count = 1);
        //IEnumerable<Product> GetNewReleases(int count = 8);

        // 个性化推荐
        IEnumerable<Product> GetPersonalizedRecommendations(string userId, int count = 1);

        // 基于内容的推荐
        IEnumerable<Product> GetRelatedProducts(int productId, int count = 1);

        // 基于类别的推荐
        IEnumerable<Product> GetProductsByCategory(int categoryId, int count = 1);

        // 记录用户活动
        void RecordUserActivity(string userId, int productId, ActivityType activityType, int? rating = null);

        // 生成产品关联度数据
        void GenerateProductRelations();


    }
}
