using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display(Name = "List Price")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }


        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }

        // 新增库存字段
        [Required]
        [Range(0, 10000)]
        [Display(Name = "Stock quantity")]//库存数量;Количество на складе
        public int StockQuantity { get; set; }

        [Display(Name = "Low inventory warning threshold")]//低库存警告阈值;Порог предупреждения о низком уровне запасов
        [Range(1, 1000)]
        public int LowStockThreshold { get; set; } = 10;

        [Display(Name = "Cumulative sales")]//累计销量;Совокупные продажи
        public int TotalSaleCount { get; set; } = 0;

        [Display(Name = "Last restocked date")]//上次补货日期;Дата последнего пополнения
        public DateTime? LastRestockDate { get; set; }

        //以下数据用于推荐系统
        public int ViewCount { get; set; } = 0;

        //public int SalesCount { get; set; } = 0;

        //public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
