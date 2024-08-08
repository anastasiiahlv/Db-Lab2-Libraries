using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryDomain.Model;
using LibraryInfrastructure;
using LibraryInfrastructure.Models;

namespace LibraryInfrastructure.Controllers
{
    public class LibrariesController : Controller
    {
        private readonly DblibraryContext _context;

        public LibrariesController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: Libraries
        public async Task<IActionResult> Index()
        {
            var dblibraryContext = _context.Libraries.Include(l => l.Director).Include(l => l.Type);
            return View(await dblibraryContext.ToListAsync());
        }

        // GET: Libraries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries
                .Include(l => l.Director)
                .Include(l => l.Type)
                .Include(l => l.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (library == null)
            {
                return NotFound();
            }

            var bookTitles = await _context.Books.FromSqlInterpolated($@"
                SELECT b.Title
                FROM Books AS b
                INNER JOIN LibrariesBooks AS lb ON b.Id = lb.BookId
                WHERE lb.LibraryId = {id}
            ").Select(b => b.Title).ToListAsync();

            ViewBag.BookTitles = bookTitles;
            return View(library);
        }

        // GET: Libraries/Create
        public IActionResult Create()
        {
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Email");
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Name");
            return View();
        }

        // POST: Libraries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,TypeId,DirectorId,Id")] Library library)
        {
            _context.Add(library);
            await _context.SaveChangesAsync();

            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Email", library.DirectorId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Name", library.TypeId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Libraries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries.FindAsync(id);
            if (library == null)
            {
                return NotFound();
            }
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Email", library.DirectorId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Name", library.TypeId);
            return View(library);
        }

        // POST: Libraries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Address,TypeId,DirectorId,Id")] Library library)
        {
            if (id != library.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(library);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibraryExists(library.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
    
            ViewData["DirectorId"] = new SelectList(_context.Directors, "Id", "Email", library.DirectorId);
            ViewData["TypeId"] = new SelectList(_context.Types, "Id", "Name", library.TypeId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Libraries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries
                .Include(l => l.Director)
                .Include(l => l.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (library == null)
            {
                return NotFound();
            }

            return View(library);
        }

        // POST: Libraries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var library = await _context.Libraries.FindAsync(id);
            if (library != null)
            {
                var libraryRemoval = new LibraryRemoval(_context, library);
                libraryRemoval.DeleteLibrary();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryExists(int id)
        {
            return _context.Libraries.Any(e => e.Id == id);
        }
    }
}
