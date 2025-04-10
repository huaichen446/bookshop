using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    // 产品间的关联度模型
    public class ProductRelation
    {
        public int Id { get; set; }
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }
        public double RelationScore { get; set; }
        public Product Product1 { get; set; }
        public Product Product2 { get; set; }

    }
}
