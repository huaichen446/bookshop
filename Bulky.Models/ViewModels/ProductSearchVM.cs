using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductSearchVM
    {
        // Search properties
        public string? SearchTerm { get; set; }
        public string? SearchField { get; set; } // For searching by specific field (Author, Title, ISBN)

        // Filter properties
        public int? CategoryId { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }

        // Sorting
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }

        // Results
        public IEnumerable<Product> Products { get; set; }

        // Drop-down lists for UI
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SearchFieldList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SortByList { get; set; }

        public ProductSearchVM()
        {
            // Default values
            Products = new List<Product>();
            SortDescending = false;

            //// Initialize CategoryList
            //CategoryList = new List<SelectListItem>();

            // Initialize search field options
            SearchFieldList = new List<SelectListItem>
            {
                new SelectListItem { Text = "All Fields", Value = "All" },
                new SelectListItem { Text = "Title", Value = "Title" },
                new SelectListItem { Text = "Author", Value = "Author" },
                new SelectListItem { Text = "ISBN", Value = "ISBN" }
            };

            // Initialize sort by options
            SortByList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Title", Value = "Title" },
                new SelectListItem { Text = "Price", Value = "Price" },
                new SelectListItem { Text = "Author", Value = "Author" }
            };
        }

    }
}
