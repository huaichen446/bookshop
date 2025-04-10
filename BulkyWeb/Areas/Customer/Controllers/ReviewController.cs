using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using System;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: 创建评论表单||Создание формы для комментариев
        public IActionResult Create(int productId)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == productId);
            if (product == null) return NotFound();

            // 创建并初始化模型
            Review model = new Review
            {
                ProductId = productId
            };

            ViewBag.ProductId = productId;
            ViewBag.ProductTitle = product.Title;

            return View(model);
        }

        // POST: 提交评论||Отправить комментарий
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Review review)
        {
            Console.WriteLine("Debug: Create method called"); ;
            Console.WriteLine($"ProductId: {review.ProductId}");
            Console.WriteLine($"Title: {review.Title}");
            Console.WriteLine($"Content: {review.Content}");
            Console.WriteLine($"Rating: {review.Rating}");

            // 移除导航属性的验证错误
            ModelState.Remove("Product");
            ModelState.Remove("ApplicationUser");
            ModelState.Remove("ApplicationUserId"); // 这个稍后会设置

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Debug: ModelState is invalid. Errors:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        Console.WriteLine($"Field: {key}");
                        foreach (var error in state.Errors)
                        {
                            Console.WriteLine($"  - Error: {error.ErrorMessage}");
                            if (error.Exception != null)
                            {
                                Console.WriteLine($"  - Exception: {error.Exception.Message}");
                            }
                        }
                    }
                }

                // 添加一个全局错误信息到视图
                ModelState.AddModelError(string.Empty, "请检查表单填写是否完整");
            }
            else
            {
                Console.WriteLine("Debug: ModelState is valid");
            }

            if (ModelState.IsValid)
            {
                review.ApplicationUserId = userId;
                review.CreatedDate = DateTime.Now;
                review.IsApproved = false; // 默认需要审核|| Аудит требуется по умолчанию

                _unitOfWork.Review.Add(review);
                _unitOfWork.Save();

                TempData["success"] = "Comment submitted, awaiting review";
                //return RedirectToAction("Details", "Home", new { id = review.ProductId });
            }

            var product = _unitOfWork.Product.Get(u => u.Id == review.ProductId);
            ViewBag.ProductId = review.ProductId;
            ViewBag.ProductTitle = product.Title;
           
            return View(review);
        }
    }
}
