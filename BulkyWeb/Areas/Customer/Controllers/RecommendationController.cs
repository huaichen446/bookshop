using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Models;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // 需要登录才能访问
    public class RecommendationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecommendationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            // 如果用户已登录，提供个性化推荐
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            RecommendationVM recommendationVM = new()
            {
                PersonalizedProducts = _unitOfWork.Recommendation.GetPersonalizedRecommendations(userId, 2),
                PopularProducts = _unitOfWork.Recommendation.GetPopularProducts(2)
            };

            return View(recommendationVM);

        }
    }
}