using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC.Models;
using MyMvcProject.Data;

namespace MVC.Controllers
{
    public class waiting_listController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();

        // GET: waiting_list
        public ActionResult Index()
        {
            return View(db.waiting_Lists.ToList());
        }

        // GET: waiting_list/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            waiting_list waiting_list = db.waiting_Lists.Find(id);
            if (waiting_list == null)
            {
                return HttpNotFound();
            }
            return View(waiting_list);
        }

        // GET: waiting_list/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: waiting_list/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,book_name,date,email")] waiting_list waiting_list)
        {
            if (ModelState.IsValid)
            {
                db.waiting_Lists.Add(waiting_list);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(waiting_list);
        }

        // GET: waiting_list/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            waiting_list waiting_list = db.waiting_Lists.Find(id);
            if (waiting_list == null)
            {
                return HttpNotFound();
            }
            return View(waiting_list);
        }

        // POST: waiting_list/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "name,book_name,date,email")] waiting_list waiting_list)
        {
            if (ModelState.IsValid)
            {
                db.Entry(waiting_list).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(waiting_list);
        }

        // GET: waiting_list/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            waiting_list waiting_list = db.waiting_Lists.Find(id);
            if (waiting_list == null)
            {
                return HttpNotFound();
            }
            return View(waiting_list);
        }

        // POST: waiting_list/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            waiting_list waiting_list = db.waiting_Lists.Find(id);
            db.waiting_Lists.Remove(waiting_list);
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
        [HttpPost]
        public ActionResult AddToWaitingList(int bookId)
        {
            var userName = Session["UserName"]?.ToString();
            var user = db.users.FirstOrDefault(u => u.name == userName);
            var book = db.books.FirstOrDefault(b => b.book_id == bookId);

            if (user == null || book == null)
            {
                TempData["ErrorMessage"] = "משתמש או ספר לא נמצאו.";
                return RedirectToAction("BuyBorrowBook", "books");
            }

            var waitingEntry = new waiting_list
            {
                name = user.name,
                book_name = book.book_name,
                email = user.email,
                date = DateTime.Now
            };

            db.waiting_Lists.Add(waitingEntry);
            db.SaveChanges();

            TempData["SuccessMessage"] = $"נכנסת לרשימת ההמתנה עבור הספר {book.book_name}.";
            return RedirectToAction("BuyBorrowBook", "books");
        }

    }
}
