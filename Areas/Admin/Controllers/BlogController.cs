using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PW.Models;
using System.Security.Claims;

namespace PW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // ===========================
        // INDEX
        // ===========================
        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .Include(b => b.Category)
                .Include(b => b.Author)
                .OrderByDescending(b => b.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return View(blogs);
        }

        // ===========================
        // DETAILS
        // ===========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _context.Blogs
                .Include(b => b.Category)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
                return NotFound();

            return View(blog);
        }

        // ===========================
        // CREATE
        // ===========================
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(
                _context.BlogCategories,
                "Id",
                "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(
                    _context.BlogCategories,
                    "Id",
                    "Name",
                    blog.CategoryId);

                return View(blog);
            }

            blog.CreatedAt = DateTime.Now;

            blog.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Blogs.Add(blog);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // EDIT
        // ===========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
                return NotFound();

            ViewBag.CategoryId = new SelectList(
                _context.BlogCategories,
                "Id",
                "Name",
                blog.CategoryId);

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blog blog)
        {
            if (id != blog.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(
                    _context.BlogCategories,
                    "Id",
                    "Name",
                    blog.CategoryId);

                return View(blog);
            }

            try
            {
                var oldBlog = await _context.Blogs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (oldBlog == null)
                    return NotFound();

                // Giữ nguyên Author và CreatedAt
                blog.AuthorId = oldBlog.AuthorId;
                blog.CreatedAt = oldBlog.CreatedAt;

                blog.UpdatedAt = DateTime.Now;

                _context.Update(blog);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(blog.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // DELETE
        // ===========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _context.Blogs
                .Include(b => b.Category)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog != null)
            {
                _context.Blogs.Remove(blog);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // CHECK EXIST
        // ===========================
        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}