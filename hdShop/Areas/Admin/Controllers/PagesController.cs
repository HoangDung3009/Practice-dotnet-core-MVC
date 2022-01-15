using hdShop.Context;
using hdShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hdShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly ShopDbContext _db;

        public PagesController(ShopDbContext _db)
        {
            this._db = _db;
        }
        //GET: admin/pages/
        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pg = from p in _db.Pages orderby p.Sorting select p;
            List<Page> pageList = await pg.ToListAsync();
            return View(pageList);
        }

        //GET: admin/pages/details
        public async Task<IActionResult> Details(int id)
        {
            Page page = await _db.Pages.FirstOrDefaultAsync(x => x.Id == id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        //POST: admin/pages/create
        public IActionResult Create()
        {
            return View();
        }

        //POST: admin/pages/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
                if (ModelState.IsValid)
                {
                    page.Slug = page.Title.ToLower().Replace(" ", "-");
                    page.Sorting = 100;

                    var slug = await _db.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                    if (slug != null)
                    {
                        ModelState.AddModelError("", "The page has been existed !!");
                        return View(page);
                    }

                    _db.Pages.Add(page);
                    await _db.SaveChangesAsync();

                    TempData["Success"] = "The page has been added !!";

                    return RedirectToAction("Index");
                }

                return View(page);
        }

        //GET admin/pages/edit
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await _db.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        //POST: admin/pages/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(" ", "-");

                var slug = await _db.Pages.Where(x => x.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page has been existed !!");
                    return View(page);
                }

                _db.Pages.Update(page);
                await _db.SaveChangesAsync();

                TempData["Success"] = "The page has been edited !!";

                return RedirectToAction("Edit", new {id = page.Id });
            }

            return View(page);
        }

        //GET admin/pages/edit
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await _db.Pages.FindAsync(id);
            if (page == null)
            {
                TempData["Error"] = "This page does not existed !!";
            }
            else
            {
                _db.Pages.Remove(page);
                await _db.SaveChangesAsync();

                TempData["Success"] = "This page has been deleted !!";
            }
            return View(page);
        }
    }
}
