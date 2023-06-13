using DataAccessLayer.Context;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Shop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart

        DataContext db = new DataContext();

        public ActionResult Index(decimal? Tutar)
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var kullanici = db.Users.FirstOrDefault(x => x.Email==username);
                var model = db.Carts.Where(x=> x.UserId == kullanici.Id.ToString()).ToList();
                var kullanici_id = db.Carts.FirstOrDefault(x => x.UserId == kullanici.Id.ToString());
                if (model != null)
                {
                    if (kullanici_id == null)
                    {
                        ViewBag.Tutar = "Sepetinizde Ürün Bulunmamaktadır";
                    }
                    else if (kullanici_id != null)
                    {
                        Tutar = db.Carts.Where(x => x.UserId == kullanici_id.UserId.ToString()).Sum(x=>x.Product.Price*x.Quantity);//toplam fiyat
                        ViewBag.Tutar = "Toplam Tutar="+Tutar+"TL";
                    }
                    return View(model);
                }
            }
            return HttpNotFound();
        }
        public ActionResult AddCart(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = db.Users.FirstOrDefault(x=>x.Email == kullaniciadi);
                var u = db.Products.Find(id);
                var sepet = db.Carts.FirstOrDefault(x=>x.UserId==model.Id.ToString() && x.ProductId == id);

                if (model != null)
                {
                    if (sepet !=null)
                    {
                        sepet.Quantity++;
                        sepet.Price = u.Price*sepet.Quantity;
                        db.SaveChanges();
                        return RedirectToAction("Index","Cart");
                    }
                    var s = new Cart
                    {
                        UserId = model.Id.ToString(),
                        ProductId = u.Id,
                        Quantity = 1,
                        Price = u.Price,
                        Date = DateTime.Now
                    };
                    db.Carts.Add(s);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Cart");
                }
            }
            return View();
        }

        public ActionResult TotalCount(int? count)
        {
            if (User.Identity.IsAuthenticated)
            {
                var model = db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
                count = db.Carts.Where(x=>x.UserId==model.Id.ToString()).Count();//sepette eklenen ürün sayısı
                ViewBag.count = count;
                if (count==0)
                {
                    ViewBag.count = "";
                }
            }
            return PartialView();
        }

        public void DinamikMiktar(int id,int miktari)
        {
            var model = db.Carts.Find(id);
            model.Quantity = miktari;
            model.Price = model.Price * model.Quantity;
            db.SaveChanges();
        }

        public ActionResult azalt(int id)
        {
            var model = db.Carts.Find(id);
            if (model.Quantity==1)
            {
                db.Carts.Remove(model);
                db.SaveChanges();
            }
            model.Quantity--;
            model.Price = model.Price * model.Quantity;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult arttir(int id)
        {
            var model = db.Carts.Find(id);
            model.Quantity++;
            model.Price = model.Price * model.Quantity;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var sil = db.Carts.Find(id);
            db.Carts.Remove(sil);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteRange()
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullanici = User.Identity.Name;
                var model = db.Users.FirstOrDefault(x=> x.Email==kullanici);
                var sil = db.Carts.Where(x=>x.UserId == model.Id.ToString());
                db.Carts.RemoveRange(sil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }
        public ActionResult Buy()
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullanici = User.Identity.Name;
                var model = db.Users.FirstOrDefault(x => x.Email == kullanici);
                var sil = db.Carts.Where(x => x.UserId == model.Id.ToString());
                db.Carts.RemoveRange(sil);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return HttpNotFound();
        }
    }
}