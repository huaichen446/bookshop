using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    //库存管理控制器
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class InventoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. 库存概览页面
        public IActionResult Index()
        {
            // 获取所有产品的库存信息
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(products);
        }

        // 2. 低库存产品页面
        public IActionResult LowStock()
        {
            var products = _unitOfWork.Product.GetLowStockProducts();
            return View(products);
        }

        // 3. 产品库存历史记录
        public IActionResult StockHistory(int id)
        {
            var product = _unitOfWork.Product.Get(p => p.Id == id, includeProperties: "Category");
            if (product == null)
                return NotFound();

            var stockHistories = _unitOfWork.StockHistory.GetProductHistory(id);

            var model = new ProductStockViewModel
            {
                Product = product,
                StockHistories = stockHistories
            };

            return View(model);
        }

        // 4. 补货操作
        [HttpGet]
        public IActionResult Restock(int id)
        {
            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product == null)
                return NotFound();

            var model = new RestockViewModel
            {
                ProductId = product.Id,
                ProductName = product.Title,
                CurrentStock = product.StockQuantity,
                QuantityToAdd = 0
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restock(RestockViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = _unitOfWork.Product.Get(p => p.Id == model.ProductId);
                if (product == null)
                    return NotFound();

                // 更新产品库存
                product.StockQuantity += model.QuantityToAdd;
                product.LastRestockDate = DateTime.Now;
                _unitOfWork.Product.Update(product);

                // 添加库存历史记录
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var stockHistory = new StockHistory
                {
                    ProductId = model.ProductId,
                    QuantityChanged = model.QuantityToAdd,
                    Action = "Restock",
                    ApplicationUserId = userId,
                    Notes = model.Notes
                };

                _unitOfWork.StockHistory.Add(stockHistory);
                _unitOfWork.Save();

                TempData["success"] = "Инвентарь успешно обновлен";//库存更新成功
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 5. 库存调整
        [HttpGet]
        public IActionResult Adjust(int id)
        {
            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product == null)
                return NotFound();

            var model = new AdjustStockViewModel
            {
                ProductId = product.Id,
                ProductName = product.Title,
                CurrentStock = product.StockQuantity,
                NewStockQuantity = product.StockQuantity
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Adjust(AdjustStockViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = _unitOfWork.Product.Get(p => p.Id == model.ProductId);
                if (product == null)
                    return NotFound();

                int quantityChanged = model.NewStockQuantity - product.StockQuantity;

                // 更新库存
                product.StockQuantity = model.NewStockQuantity;
                _unitOfWork.Product.Update(product);

                // 添加库存变动记录
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var stockHistory = new StockHistory
                {
                    ProductId = model.ProductId,
                    QuantityChanged = quantityChanged,
                    Action = "Adjustment",
                    ApplicationUserId = userId,
                    Notes = model.Reason
                };

                _unitOfWork.StockHistory.Add(stockHistory);
                _unitOfWork.Save();

                TempData["success"] = "Корректировка инвентаря прошла успешно";//库存调整成功
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 6. 阈值设置
        [HttpGet]
        public IActionResult SetThreshold(int id)
        {
            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product == null)
                return NotFound();

            var model = new ThresholdViewModel
            {
                ProductId = product.Id,
                ProductName = product.Title,
                CurrentThreshold = product.LowStockThreshold,
                NewThreshold = product.LowStockThreshold
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetThreshold(ThresholdViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = _unitOfWork.Product.Get(p => p.Id == model.ProductId);
                if (product == null)
                    return NotFound();

                product.LowStockThreshold = model.NewThreshold;
                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();

                TempData["success"] = "Порог инвентаризации успешно обновлен";//库存阈值更新成功
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

    }
}
