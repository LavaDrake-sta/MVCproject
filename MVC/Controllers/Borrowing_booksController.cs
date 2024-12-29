using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class Borrowing_booksController : Controller
    {
        private MVC_PROJECTEntities1 db = new MVC_PROJECTEntities1();

        // GET: Borrowing_books
        public ActionResult Index()
        {
            return View(db.Borrowing_books.ToList());
        }

        // GET: Borrowing_books/Details/5
        public ActionResult Details(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing_books borrowing_books = db.Borrowing_books.Find(id);
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

        // POST: Borrowing_books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "book_id,book_name,category,language,publisher,Publication_date,available,price")] Borrowing_books borrowing_books)
        {
            if (ModelState.IsValid)
            {
                db.Borrowing_books.Add(borrowing_books);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(borrowing_books);
        }

        // GET: Borrowing_books/Edit/5
        public ActionResult Edit(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing_books borrowing_books = db.Borrowing_books.Find(id);
            if (borrowing_books == null)
            {
                return HttpNotFound();
            }
            return View(borrowing_books);
        }

        // POST: Borrowing_books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Borrowing_books/Delete/5
        public ActionResult Delete(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing_books borrowing_books = db.Borrowing_books.Find(id);
            if (borrowing_books == null)
            {
                return HttpNotFound();
            }
            return View(borrowing_books);
        }

        // POST: Borrowing_books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(float id)
        {
            Borrowing_books borrowing_books = db.Borrowing_books.Find(id);
            db.Borrowing_books.Remove(borrowing_books);
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
