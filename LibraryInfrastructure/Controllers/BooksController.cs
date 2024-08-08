using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryDomain.Model;
using LibraryInfrastructure;

namespace LibraryInfrastructure.Controllers
{
    public class BooksController : Controller
    {
        private readonly DblibraryContext _context;

        public BooksController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var dblibraryContext = _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Libraries);
            return View(await dblibraryContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Libraries)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            var authors = _context.Authors.Select(a => new { Id = a.Id, FullName = a.Name + " " + a.Surname }).ToList();
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name");
            ViewData["Libraries"] = new MultiSelectList(_context.Libraries, "Id", "Name");
            ViewData["Authors"] = new MultiSelectList(authors, "Id", "FullName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublisherId, Libraries, Authors, Title, Description, Id")] Book book, int[] Libraries, int[] Authors)
        {

            if (Libraries != null)
            {
                foreach (var libraryId in Libraries)
                {
                    var library = await _context.Libraries.FindAsync(libraryId);
                    if (library != null)
                    {
                        book.Libraries.Add(library);
                    }
                }
            }

            if (Authors != null)
            {
                foreach (var authorId in Authors)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null)
                    {
                        book.Authors.Add(author);
                    }
                }
            }
            _context.Add(book);
            await _context.SaveChangesAsync();

            var authors = _context.Authors.Select(a => new { Id = a.Id, FullName = a.Name + " " + a.Surname }).ToList();

            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            ViewData["Libraries"] = new MultiSelectList(_context.Libraries, "Id", "Name", book.Libraries.Select(c => c.Id));
            ViewData["Authors"] = new MultiSelectList(authors, "Id", "FullName");
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Libraries)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var authors = _context.Authors.Select(a => new { Id = a.Id, FullName = a.Name + " " + a.Surname }).ToList();

            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            ViewData["Libraries"] = new MultiSelectList(_context.Libraries, "Id", "Name", book.Libraries.Select(l => l.Id));
            ViewData["Authors"] = new MultiSelectList(authors, "Id", "FullName", book.Authors.Select(a => a.Id));
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PublisherId, Libraries, Authors, Title, Description, Id")] Book book, int[] Libraries, int[] Authors)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();

                var bookInDb = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Libraries)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);

                //bookInDb?.Libraries.Clear();
                foreach (var libraryId in Libraries)
                {
                    var library = await _context.Libraries.FindAsync(libraryId);
                    if (library != null)
                    {
                        bookInDb?.Libraries.Add(library);
                    }
                }

                //bookInDb?.Authors.Clear();
                foreach (var authorId in Authors)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null)
                    {
                        bookInDb?.Authors.Add(author);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Libraries)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
