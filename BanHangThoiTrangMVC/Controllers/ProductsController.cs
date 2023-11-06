using BanHangThoiTrangMVC.Models;
using BanHangThoiTrangMVC.Models.EF;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Detail(string alias, int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                if(item.ViewCount == 1000)
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

    }
}