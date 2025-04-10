using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models.ViewModels;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    //
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class SalesAnalyticsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesAnalyticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. 销售概览
        public IActionResult Index()
        {
            return View();
        }

        // 2. 畅销书籍
        public IActionResult BestSellers()
        {
            var products = _unitOfWork.Product.GetBestSellingProducts(20);
            return View(products);
        }

        // 3. 获取销售趋势数据 (API 端点)
        [HttpGet]
        public IActionResult SalesTrendData(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-3);

            if (!endDate.HasValue)
                endDate = DateTime.Now;

            // 从订单头和订单明细获取销售数据
            var orders = _unitOfWork.OrderHeader.GetAll(
                filter: o => o.OrderStatus != SD.StatusCancelled &&
                       o.OrderDate >= startDate &&
                       o.OrderDate <= endDate);

            // 按日期分组计算每天的销售总额
            var salesByDay = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(o => o.OrderTotal)
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Json(salesByDay);
        }

        // 4. 获取产品类别销售占比数据
        [HttpGet]
        public IActionResult CategorySalesData()
        {
            // 获取所有已完成的订单详情
            var orderDetails = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product.Category");

            // 按类别分组
            var salesByCategory = orderDetails
                .GroupBy(od => od.Product.Category.Name)
                .Select(g => new {
                    Category = g.Key,
                    Revenue = g.Sum(od => od.Price * od.Count),
                    Count = g.Sum(od => od.Count)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            return Json(salesByCategory);
        }

        // 5. 库存周转率分析
        public IActionResult InventoryTurnover()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category");

            var model = products.Select(p => new ProductTurnoverViewModel
            {
                ProductId = p.Id,
                Title = p.Title,
                Category = p.Category.Name,
                CurrentStock = p.StockQuantity,
                TotalSales = p.TotalSaleCount,
                // 简单计算周转率，实际可能需要更复杂的公式
                TurnoverRate = p.StockQuantity > 0 ? (double)p.TotalSaleCount / p.StockQuantity : 0
            }).ToList();

            return View(model);
        }

    }
}
