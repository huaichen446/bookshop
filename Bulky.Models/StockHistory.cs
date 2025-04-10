using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class StockHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int QuantityChanged { get; set; }  // 正值表示添加库存，负值表示减少库存

        [Required]
        public string Action { get; set; }  // "Restock"(补货), "Sale"(销售), "Adjustment"(调整)

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
