using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetDrinks.Data;
using DotNetDrinks.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
// Add reference to authorization nuget package
using Microsoft.AspNetCore.Authorization;

namespace DotNetDrinks.Controllers
{
    // To make all methods in this controller accessible to authenticated users ONLY add:
    // [Authorize]
    // To make all methods in this controller accessible to authenticated users with Admin roles ONLY add:
    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Brand).Include(p => p.Category).OrderBy(p => p.Name);
            return View("Index", await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands.OrderBy(b => b.Name), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name");
            return View("Create");
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,BrandId,CategoryId")] Product product, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                // upload photo and attach to the new product if any
                if (Image != null)
                {
                    var filePath = Path.GetTempFileName(); // get image from cache
                    var fileName = Guid.NewGuid() + "-" + Image.FileName; // add unique id as prefix to file name
                    var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\products\\" + fileName;

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await Image.CopyToAsync(stream);
                    }

                    // add unique Image file name to the new product object before saving
                    product.Image = fileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands.OrderBy(b => b.Name), "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Stock,BrandId,CategoryId")] Product product, IFormFile Image, string CurrentImage)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // upload photo and attach to the new product if any
                    if (Image != null)
                    {
                        var filePath = Path.GetTempFileName(); // get image from cache
                        var fileName = Guid.NewGuid() + "-" + Image.FileName; // add unique id as prefix to file name
                        var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\products\\" + fileName;

                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        // add unique Image file name to the new product object before saving
                        product.Image = fileName;
                    }
                    else
                    {
                        // keep current image from getting wiped out if nothing new uploaded
                        product.Image = CurrentImage;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            // remove image file if any
            var Image = product.Image;
            // delete file from wwwroot\img\products first
                

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
