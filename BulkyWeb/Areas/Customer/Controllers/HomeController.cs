using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(ProductSearchVM searchVM)
        {
            // Initialize the view model if it's null
            if (searchVM == null)
            {
                searchVM = new ProductSearchVM();
            }

            // Populate category dropdown
            searchVM.CategoryList = _unitOfWork.Category.GetAll().Select(c =>
                new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

            // Execute search and filtering
            searchVM.Products = _unitOfWork.Product.SearchAndFilter(
                searchVM.SearchTerm,
                searchVM.SearchField,
                searchVM.CategoryId,
                searchVM.MinPrice,
                searchVM.MaxPrice,
                searchVM.SortBy,
                searchVM.SortDescending);

            //HomeVM homeVM = new()
            //{
            //    Products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages"),
            //    Categories = _unitOfWork.Category.GetAll(),
            //    PopularProducts = _unitOfWork.Recommendation.GetPopularProducts(),
            //    NewProducts = _unitOfWork.Recommendation.GetNewProducts()
            //};

            //// 使用 ViewBag 传递推荐数据
            //ViewBag.PopularProducts = _unitOfWork.Recommendation.GetPopularProducts();
            //ViewBag.NewProducts = _unitOfWork.Recommendation.GetNewProducts();
            //ViewBag.Categories = _unitOfWork.Category.GetAll();

            return View(searchVM);
        }

        //public IActionResult Index()
        //{

        //    IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
        //    return View(productList);
        //}

        public IActionResult Details(int productId)
        {
            // 调试信息
            System.Diagnostics.Debug.WriteLine($"接收到的产品ID: {productId}");
            var product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages");
            System.Diagnostics.Debug.WriteLine($"查询到的产品: {product?.Title ?? "null"}");

            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = productId
            };

            // 更新浏览计数
            if (cart.Product != null)
            {
                cart.Product.ViewCount++;
                _unitOfWork.Product.Update(cart.Product);
                _unitOfWork.Save();
            }

            // 获取已审核的评论
            var reviews = _unitOfWork.Review.GetAllByProduct(productId);
            ViewBag.Reviews = reviews;

            // 获取相关推荐
           // ViewBag.RelatedProducts = _unitOfWork.Recommendation.GetRelatedProducts(productId);

            // 记录用户浏览行为
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _unitOfWork.Recommendation.RecordUserActivity(userId, productId, ActivityType.View);
            }

            //// 获取平均评分
            ViewBag.AverageRating = _unitOfWork.Review.GetAverageRating(productId);

            //// 获取相关推荐并通过ViewBag传递给视图
            //ViewBag.RelatedProducts = _unitOfWork.Recommendation.GetRelatedProducts(productId);

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart) 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId= userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == userId &&
            u.ProductId==shoppingCart.ProductId);

            if (cartFromDb != null) {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else {
                //add cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";

           


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}