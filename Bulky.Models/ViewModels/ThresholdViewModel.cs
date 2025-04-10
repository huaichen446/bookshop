using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ThresholdViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentThreshold { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Please enter a number between 1-1000")]
        [Display(Name = "Новый порог предупреждения")]//新警告阈值
        public int NewThreshold { get; set; }

    }
}
