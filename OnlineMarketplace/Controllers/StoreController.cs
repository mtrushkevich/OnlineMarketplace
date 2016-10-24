using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineMarketplace.DAL;
using OnlineMarketplace.Models;
using OnlineMarketplace.ViewModel;

namespace OnlineMarketplace.Controllers
{
    public class StoreController : Controller
    {
        private MarketplaceContext db = new MarketplaceContext();

        // GET: Store
        public ActionResult Index()
        {
            return View(db.Stores.ToList());
        }

        // GET: Store/ProductList/5
        public ActionResult ProductList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // GET: Store/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Store/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Title,OpenTime,CloseTime")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(store);
        }

        // GET: Store/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores
                .Include(s => s.Products)
                .Single(s => s.StoreId == id);
            if (store == null)
            {
                return HttpNotFound();
            }
            PopulateAssignedProductsData(store);
            return View(store);
        }

        // POST: Store/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int?id, string[] selecteProducts)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store storeToUpdate = db.Stores
                .Include(s => s.Products)
                .Where(s => s.StoreId == id)
                .Single();
            if (TryUpdateModel(storeToUpdate, "", new string[] {"ProductId,Title,OpenTime,CloseTime"})) ;
            {
                try
                {
                    UpdateStoreProducts(selecteProducts, storeToUpdate);

                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateAssignedProductsData(storeToUpdate);
            return View(storeToUpdate);
        }

        // GET: Store/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Store/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Store store = db.Stores.Find(id);
            db.Stores.Remove(store);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateAssignedProductsData(Store store)
        {
            var allProducts = db.Products;
            var storeProducts = new HashSet<int>(store.Products.Select(p => p.ProductId));
            var viewModel = new List<AssignedProductData>();
            foreach (var product in allProducts)
            {
                viewModel.Add(new AssignedProductData
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Assigned = storeProducts.Contains(product.ProductId)
                });
            }
            ViewBag.Products = viewModel;
        }

        private void UpdateStoreProducts(string[] selectedProducts, Store storeToUpdate)
        {
            if (selectedProducts == null)
            {
                storeToUpdate.Products = new List<Product>();
                return;
            }

            var selectedProductsHS = new HashSet<string>(selectedProducts);
            var storeProducts = new HashSet<int>(storeToUpdate.Products.Select(p => p.ProductId));

            foreach (var product in db.Products)
            {
                if (selectedProductsHS.Contains(product.ProductId.ToString()))
                {
                    if (!storeProducts.Contains(product.ProductId))
                    {
                        storeToUpdate.Products.Add(product);
                    }
                }
                else
                {
                    if (storeProducts.Contains(product.ProductId))
                    {
                        storeToUpdate.Products.Remove(product);
                    }
                }

            }

        }
    }
}
