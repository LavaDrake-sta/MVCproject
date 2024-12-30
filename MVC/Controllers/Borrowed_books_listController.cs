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
    public class Borrowed_books_listController : Controller
    {
        private MvcProjectContext db = new MvcProjectContext();

        // GET: Borrowed_books_list
        public ActionResult Index()
        {
            return View(db.borrowed_Books_Lists.ToList());
        }

        // GET: Borrowed_books_list/Details/5
        public ActionResult Details(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowed_books_list borrowed_books_list = db.borrowed_Books_Lists.Find(id);
            if (borrowed_books_list == null)
            {
                return HttpNotFound();
            }
            return View(borrowed_books_list);
        }

        // GET: Borrowed_books_list/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Borrowed_books_list/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "book_id,book_name,category,Date_taken,return_date")] Borrowed_books_list borrowed_books_list)
        {
            if (ModelState.IsValid)
            {
                db.borrowed_Books_Lists.Add(borrowed_books_list);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(borrowed_books_list);
        }

        // GET: Borrowed_books_list/Edit/5
        public ActionResult Edit(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowed_books_list borrowed_books_list = db.borrowed_Books_Lists.Find(id);
            if (borrowed_books_list == null)
            {
                return HttpNotFound();
            }
            return View(borrowed_books_list);
        }

        // POST: Borrowed_books_list/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "book_id,book_name,category,Date_taken,return_date")] Borrowed_books_list borrowed_books_list)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrowed_books_list).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(borrowed_books_list);
        }

        // GET: Borrowed_books_list/Delete/5
        public ActionResult Delete(float? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowed_books_list borrowed_books_list = db.borrowed_Books_Lists.Find(id);
            if (borrowed_books_list == null)
            {
                return HttpNotFound();
            }
            return View(borrowed_books_list);
        }

        // POST: Borrowed_books_list/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(float id)
        {
            Borrowed_books_list borrowed_books_list = db.borrowed_Books_Lists.Find(id);
            db.borrowed_Books_Lists.Remove(borrowed_books_list);
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
