using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using LibraryDomain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;
using LibraryInfrastructure.Models;

namespace LibraryInfrastructure.Controllers
{
    public class QueriesController : Controller
    {
        private readonly string _connectionString;
        private readonly DblibraryContext _context;

        public QueriesController(DblibraryContext context)
        {
            _context = context;
            _connectionString = "Server=HOME-PC\\SQLEXPRESS; Database=DBLibrary; Trusted_Connection=True; MultipleActiveResultSets=true";
        }

        public IActionResult Index()
        {
            return View();
        }

        // Запит 1: Знайти назви бібліотек, директори яких ___ статі.
        public async Task<IActionResult> A1(string gender)
        {
            var genders = _context.Genders.Select(g => new { g.Id, g.Name }).ToList();
            ViewBag.GenderId = new SelectList(genders, "Name", "Name", gender);
            List<Library> libraries = new List<Library>();

            try
            {
                libraries = await _context.Libraries
                    .FromSqlInterpolated($@"
                    SELECT DISTINCT L.*
                    FROM Libraries L
                    INNER JOIN Directors D ON L.DirectorId = D.Id
                    INNER JOIN Genders G ON D.GenderId = G.Id
                    WHERE G.Name = {gender}")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час виконання запиту: {ex.Message}");
            }
        
            return View(libraries);
        }

        // Запит 2: Знайти назви та опис видавництв, книги яких є в ___ бібліотеці.
        public async Task<IActionResult> A2(int libraryId)
        {
            var libraries = _context.Libraries.Select(l => new { l.Id, l.Name }).ToList();
            ViewBag.LibraryId = new SelectList(libraries, "Id", "Name", libraryId);

            var publishers = await _context.Publishers
                .FromSqlInterpolated($@"
            SELECT DISTINCT P.*
            FROM Publishers P
            INNER JOIN Books B ON P.Id = B.PublisherId
            INNER JOIN LibrariesBooks LB ON B.Id = LB.BookId
            WHERE LB.LibraryId = {libraryId}")
                .ToListAsync();

            ViewBag.LibraryId = new SelectList(libraries, "Id", "Name", libraryId);
            return View(publishers);
        }

        // Запит 3: Знайти назву та адресу бібліотеки, де є книга ___.
        public async Task<IActionResult> A3(string bookTitle)
        {
            var books = _context.Books.Select(b => new { b.Id, b.Title }).ToList();
            ViewBag.BookTitle = new SelectList(books, "Title", "Title", bookTitle);

            var libraries = new List<Library>();

            if (!string.IsNullOrEmpty(bookTitle))
            {
                libraries = await _context.Libraries
                    .FromSqlInterpolated($@"
                SELECT L.*
                FROM Libraries L
                INNER JOIN LibrariesBooks LB ON L.Id = LB.LibraryId
                INNER JOIN Books B ON LB.BookId = B.Id
                WHERE B.Title = {bookTitle}")
                    .ToListAsync();
            }

            return View(libraries);
        }

        // Запит 4: Знайти назву та тип бібліотеки, директор якої має таку адресу електронної пошти ___ . 
        public async Task<IActionResult> A4(string directorEmail)
        {
            if (string.IsNullOrEmpty(directorEmail))
            {
                return View(new List<Library>());
            }

            var libraries = await _context.Libraries
                .FromSqlInterpolated($@"
            SELECT L.*
            FROM Libraries L
            INNER JOIN Directors D ON L.DirectorId = D.Id
            INNER JOIN Types T ON L.TypeId = T.Id
            WHERE D.Email = {directorEmail}")
                .Select(l => new Library
                {
                    Name = l.Name,
                    Type = l.Type
                })
                .ToListAsync();

            return View(libraries);
        }

        // Запит 5: Знайти назву та адресу бібліотеки, у якій >n  книг від ___ автора.
        public async Task<IActionResult> A5(int authorId, int minBooks)
        {
            var authors = _context.Authors.Select(a => new { Id = a.Id, FullName = a.Name + " " + a.Surname }).ToList();
            ViewBag.AuthorId = new SelectList(authors, "Id", "FullName", authorId);

            var libraries = await _context.Libraries
                .FromSqlInterpolated($@"
            SELECT L.Name AS Name, L.Address AS Address
            FROM Libraries L
            INNER JOIN LibrariesBooks LB ON L.Id = LB.LibraryId
            INNER JOIN Books B ON LB.BookId = B.Id
            INNER JOIN AuthorsBooks AB ON B.Id = AB.BookId
            WHERE AB.AuthorId = {authorId}
            GROUP BY L.Name, L.Address
            HAVING COUNT(B.Id) > {minBooks}")
                .Select(l => new Library
                {
                    Name = l.Name,
                    Address = l.Address
                })
                .ToListAsync();

            return View(libraries);
        }

        // Запит 6: Знайти назви та адреса бібліотек, у яких є всі книги автора ___
        public async Task<IActionResult> B1(int? authorId)
        {
            var authors = _context.Authors
        .Select(a => new { a.Id, FullName = a.Name + " " + a.Surname })
        .ToList();
            ViewBag.AuthorId = new SelectList(authors, "Id", "FullName", authorId);

            if (!authorId.HasValue)
            {
                return View(new List<Library>());
            }

            var results = await _context.Libraries
                .FromSqlInterpolated($@"
            SELECT L.Name, L.Address
            FROM Libraries L
            WHERE NOT EXISTS (
                SELECT 1
                FROM Books B
                INNER JOIN AuthorsBooks AB ON B.Id = AB.BookId
                WHERE AB.AuthorId = {authorId}
                AND NOT EXISTS (
                    SELECT 1
                    FROM LibrariesBooks LB
                    WHERE LB.LibraryId = L.Id
                    AND LB.BookId = B.Id
                )
            )")
                .Select(l => new Library
                {
                    Name = l.Name,
                    Address = l.Address
                })
                .ToListAsync();

            return View(results);
        }

        //Запит 7: Визначити назви видавництв, ВСІ книги яких є в бібліотеках з типом ___ і обов'язково ще якихось.
        public async Task<IActionResult> B2(int typeId)
        {
            var types = await _context.Types.Select(t => new { t.Id, t.Name }).ToListAsync();

            ViewBag.TypeId = new SelectList(types, "Id", "Name", typeId);

            var results = await _context.Publishers
                .FromSqlInterpolated($@"
            SELECT DISTINCT P.Name
            FROM Publishers P
            WHERE EXISTS (
                SELECT 1
                FROM Books B
                INNER JOIN LibrariesBooks LB ON B.Id = LB.BookId
                INNER JOIN Libraries L ON LB.LibraryId = L.Id
                WHERE B.PublisherId = P.Id
                AND L.TypeId = {typeId}
            )
            AND EXISTS (
                SELECT 1
                FROM Books B
                INNER JOIN LibrariesBooks LB ON B.Id = LB.BookId
                INNER JOIN Libraries L ON LB.LibraryId = L.Id
                WHERE B.PublisherId = P.Id
                AND L.TypeId <> {typeId}
            )
            AND NOT EXISTS (
                SELECT 1
                FROM Books B
                WHERE B.PublisherId = P.Id
                AND NOT EXISTS (
                    SELECT 1
                    FROM LibrariesBooks LB
                    INNER JOIN Libraries L ON LB.LibraryId = L.Id
                    WHERE LB.BookId = B.Id
                    AND L.TypeId = {typeId}
                )
            )")
                .Select(p => new Publisher
                {
                    Name = p.Name
                })
                .ToListAsync();

            return View(results);
        }

        // Запит 8: Знайти імена та прізвища авторів, всі книги яких є у видавництві ___ .
        public async Task<IActionResult> B3(int publisherId)
        {
            var publishers = await _context.Publishers
                            .Select(p => new { p.Id, p.Name })
                            .ToListAsync();

            ViewBag.PublisherId = new SelectList(publishers, "Id", "Name", publisherId);

            var results = await _context.Authors
                .FromSqlInterpolated($@"
            SELECT A.Name, A.Surname
            FROM Authors A
            WHERE EXISTS (
                SELECT 1
                FROM Books B
                INNER JOIN AuthorsBooks AB ON B.Id = AB.BookId
                WHERE AB.AuthorId = A.Id
                AND B.PublisherId = {publisherId}
            )
            AND NOT EXISTS (
                SELECT 1
                FROM Books B
                INNER JOIN AuthorsBooks AB ON B.Id = AB.BookId
                WHERE AB.AuthorId = A.Id
                AND B.PublisherId <> {publisherId}
            )")
                .Select(a => new Author
                {
                    Name = a.Name,
                    Surname = a.Surname
                })
                .ToListAsync();

            return View(results);
        }

    }
}
