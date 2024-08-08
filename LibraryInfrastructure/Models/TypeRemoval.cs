using LibraryDomain.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Policy;

namespace LibraryInfrastructure.Models
{
    public class TypeRemoval
    {
        private DblibraryContext _context;
        public LibraryDomain.Model.Type Type { get; set; } = null!;

        public List<Library> TypeLibraries { get; set; }

        public TypeRemoval(DblibraryContext context, LibraryDomain.Model.Type type)
        {
            _context = context;
            Type = type;

            TypeLibraries = context.Libraries
                .Where(l => l.TypeId == type.Id)
                .ToList()!;
        }

        public void DeleteType()
        {
            var librariesInType = _context.Libraries
                .Where(l => l.TypeId == Type.Id)
                .ToList();

            foreach (var library in librariesInType)
            {
                if (library != null)
                {
                    _context.Libraries.Remove(library);
                    _context.SaveChanges();
                }
            }

            _context.Types.Remove(Type);
            _context.SaveChanges();
        }
    }
}
