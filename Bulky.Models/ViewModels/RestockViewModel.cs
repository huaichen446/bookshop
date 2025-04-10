using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class RestockViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Please enter a number between 1-10000")]
        [Display(Name = "Replenishment quantity")]//补货数量
        public int QuantityToAdd { get; set; }

        [Display(Name = "Remark")]//备注
        public string? Notes { get; set; }
    }
}
