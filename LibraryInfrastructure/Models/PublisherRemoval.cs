﻿using LibraryDomain.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LibraryInfrastructure.Models
{
    public class PublisherRemoval
    {
        private DblibraryContext _context;
        public LibraryDomain.Model.Publisher Publisher { get; set; } = null!;

        public List<Book> PublisherBooks { get; set; }

        public PublisherRemoval(DblibraryContext context, LibraryDomain.Model.Publisher publisher)
        {
            _context = context;
            Publisher = publisher;

            PublisherBooks = context.Books
                .Where(b => b.PublisherId == publisher.Id)
                .ToList()!;
        }

        public void DeletePublisher()
        {
            var booksByPublisher = _context.Books
                .Where(b => b.PublisherId == Publisher.Id)
                .ToList();

            foreach (var book in booksByPublisher)
            {
                if (book != null)
                {
                    _context.Books.Remove(book);
                    _context.SaveChanges();
                }
            }

            _context.Publishers.Remove(Publisher);
            _context.SaveChanges();
        }
    }
}
