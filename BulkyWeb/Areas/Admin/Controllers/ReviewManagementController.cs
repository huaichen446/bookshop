using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ReviewManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 所有评论列表
        public IActionResult Index()
        {
            IEnumerable<Review> reviews = _unitOfWork.Review.GetAll(includeProperties: "Product,ApplicationUser");
            return View(reviews);
        }

        // 待审核评论
        public IActionResult PendingReviews()
        {
            IEnumerable<Review> reviews = _unitOfWork.Review.GetAll(
                filter: r => !r.IsApproved,
                includeProperties: "Product,ApplicationUser");
            return View(reviews);
        }

        // 审核评论
        public IActionResult Approve(int id)
        {
            Review review = _unitOfWork.Review.Get(r => r.Id == id);
            if (review == null) return NotFound();

            review.IsApproved = true;
            _unitOfWork.Review.Update(review);
            _unitOfWork.Save();

            TempData["success"] = "Comments have been moderated";
            return RedirectToAction(nameof(PendingReviews));
        }

        // 删除评论
        public IActionResult Delete(int id)
        {
            Review review = _unitOfWork.Review.Get(r => r.Id == id);
            if (review == null) return NotFound();

            _unitOfWork.Review.Remove(review);
            _unitOfWork.Save();

            TempData["success"] = "Comment deleted";
            return RedirectToAction(nameof(Index));
        }

    }
}
