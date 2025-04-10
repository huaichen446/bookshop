using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class AdjustStockViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Please enter a number between 0-10000")]
        [Display(Name = "Количество нового товара на складе")]//新库存数量
        public int NewStockQuantity { get; set; }

        [Required]
        [Display(Name = "Причины корректировки")]//调整原因
        public string Reason { get; set; }

    }
}
