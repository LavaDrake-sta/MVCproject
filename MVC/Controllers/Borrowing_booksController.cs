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
    public class Borrowing_booksController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();

        // GET: Borrowing_books
        public ActionResult Index()
        {
            return View(db.borrowing_Books.ToList());
        }

        public ActionResult Details(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing_books borrowing_books = db.borrowing_Books.Find(id);
            if (borrowing_books == null)
            {
                return HttpNotFound();
            }
            return View(borrowing_books);
        }

        // GET: Borrowing_books/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "book_id,book_name,category,language,publisher,Publication_date,available,price")] Borrowing_books borrowing_books)
        {
            if (ModelState.IsValid)
            {
                db.borrowing_Books.Add(borrowing_books);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(borrowing_books);
        }

        public ActionResult Edit(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing_books borrowing_books = db.borrowing_Books.Find(id);
            if (borrowing_books == null)
            {
                return HttpNotFound();
            }
            return View(borrowing_books);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "book_id,book_name,category,language,publisher,Publication_date,available,price")] Borrowing_books borrowing_books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrowing_books).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(borrowing_books);
        }

        public ActionResult Delete(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing_books borrowing_books = db.borrowing_Books.Find(id);
            if (borrowing_books == null)
            {
                return HttpNotFound();
            }
            return View(borrowing_books);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(float id)
        {
            Borrowing_books borrowing_books = db.borrowing_Books.Find(id);
            db.borrowing_Books.Remove(borrowing_books);
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
    }
}