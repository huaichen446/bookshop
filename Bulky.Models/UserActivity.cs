using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public ActivityType ActivityType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int? Rating { get; set; } // 评分 1-5
    }

    public enum ActivityType
    {
        View, // 用户查看了某个内容（如商品、页面等）
        AddToCart,// 用户将商品添加到购物车
        Purchase, // 用户完成了购买
        Review// 用户对商品进行了评价或评论
    }

}
