using BanHangThoiTrangMVC.Models;
using BanHangThoiTrangMVC.Models.EF;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(string Searchtext, int? id)
        {
            /*var items = db.Products.Where(x => x.IsActive).Take(12).ToList();*/
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            items = items.Where(x => x.IsActive).Take(12).ToList();
            /*if (id != null)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }*/
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }
            return View(items);
        }

        public ActionResult ProductCategory(string alias, int id)
        {
            var items = db.Products.Where(x => x.IsActive).Take(12).ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }
            ViewBag.CateId = id;
            return View(items);
        }

        public ActionResult Partial_ItemsByCateId()
        {
            var items = db.Products.Where(x => x.IsHome && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_ProductSale()
        {
            var items = db.Products.Where(x => x.IsSale && x.IsActive).Take(12).ToList();
            return PartialView(items);
        }

        public ActionResult Partial_Review(int id)
        {
            var item = db.Products
            .Include(p => p.ParentComments)
            .FirstOrDefault(p => p.Id == id);
            return PartialView(item);
        }

        public ActionResult Detail(string alias, int id)
        {

            var item = db.Products
            .Include(p => p.ParentComments)
            .FirstOrDefault(p => p.Id == id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                if (item.ViewCount == 1000)
                {
                    item.ViewCount = 0;
                }
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            return View(item);
        }

        public ActionResult Order(int? page)
        {
            string currentUserId = User.Identity.GetUserId();
            var items = db.Orders
                .Where(x => x.UserId == currentUserId)
                .OrderByDescending(x => x.CreateDate)
                .ToList();

            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ViewOrder(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        public ActionResult WishList(int? page)
        {

            var pageSize = 10;
            var pageNumber = page ?? 1;

            var userId = User.Identity.GetUserId();
            var likedProducts = db.LikeProducts
                .Where(l => l.UserId == userId)
                .Include(l => l.Products)
                .OrderByDescending(l => l.CreateDate)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.Page = pageNumber;
            ViewBag.PageSize = pageSize;

            return View(likedProducts);
        }

        [HttpPost]
        public ActionResult WishList(int? ProductId, LikeProduct model)
        {
            if (ModelState.IsValid) 
            {
                if (ProductId.HasValue && User.Identity.IsAuthenticated)
                {
                    var userId = User.Identity.GetUserId();
                    var existingLike = db.LikeProducts
                        .FirstOrDefault(l => l.ProductId == ProductId.Value && l.UserId == userId);

                    if (existingLike == null)
                    {
                        model.ProductId = ProductId.Value;
                        model.UserId = userId;
                        model.CreateDate = DateTime.Now;
                        model.ModifiedDate = DateTime.Now;

                        db.LikeProducts.Add(model);
                        db.SaveChanges();

                        TempData["SuccessMessage"] = "Thêm vào danh sách yêu thích thành công";
                        return RedirectToAction("Index", "Home");
                    }
                    TempData["SuccessMessage"] = "Sản phẩm đã có trong danh sách yêu thích";
                    return RedirectToAction("Index", "Home");
                }
                TempData["ErrorMessage"] = "Người dùng chưa đăng nhập hoặc sản phẩm không tồn tại";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your data.";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var item = db.LikeProducts.Find(id);
            if (item != null)
            {
                db.LikeProducts.Remove(item);
                db.SaveChanges();
                return RedirectToAction("WishList", "Products");
            }
            return Json(new { success = false, message = "Like not found." });
        }

        [HttpPost]

        public ActionResult Post(string Text, int ProductId)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    if (Text.Count() > 150)
                    {
                        
                    }
                    else
                    {
                        var currentUserId = User.Identity.GetUserId();
                        var currentUserName = User.Identity.Name;

                        var model = new ParentComment
                        {
                            user_id = currentUserId,
                            user_name = currentUserName,
                            Text = Text,
                            ProductId = ProductId,
                            Comment_Date = DateTime.Now,
                            CreateDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        };

                        db.ParentComments.Add(model);
                        db.SaveChanges();
                        TempData["CommentSuccess"] = "Bạn đã bình luận thành công";
                    }

                    
                    return RedirectToAction("Detail", "Products", new {id = ProductId });
                    
                }
                
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi gửi bình luận.");
                }
            }
            return Json(new { Success = true, msg = "Bạn cần đăng nhập để bình luận" });
            /*TempData["CommentError"] = "Bạn cần đăng nhập để bình luận";
            return RedirectToAction("Detail", "Products", new { id = ProductId });*/
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Comment(int id)
        {
            var item = db.ParentComments.Find(id);
            if (item != null)
            {
                db.ParentComments.Remove(item);
                db.SaveChanges();
                TempData["CommentSuccess"] = "Bạn đã xóa bình luận thành công";
                return RedirectToAction("Detail", "Products", new { id = item.ProductId });
            }
            return Json(new { success = false, message = "Like not found." });
        }
    }
}