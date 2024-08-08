using LibraryDomain.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LibraryInfrastructure.Models
{
    public class LibraryRemoval
    {
        private DblibraryContext _context;
        public Library Library { get; set; } = null!;

        public LibraryRemoval(DblibraryContext context, Library library)
        {
            _context = context;
            Library = library;
        }

        public void DeleteLibrary()
        {
            var booksInLibrary = _context.Books
                .Where(b => b.Libraries.Any(l => l.Id == Library.Id))
                .ToList();

            foreach (var book in booksInLibrary)
            {
                if (book != null)
                {
                    if (book.Libraries.Count == 1)
                    {
                        _context.Books.Remove(book);
                    }
                    else
                    {
                        book.Libraries.Remove(Library);
                    }
                }
            }

            _context.Libraries.Remove(Library);
            _context.SaveChanges();
        }
    }
}
