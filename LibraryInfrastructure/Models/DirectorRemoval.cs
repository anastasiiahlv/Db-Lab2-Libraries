using LibraryDomain.Model;

namespace LibraryInfrastructure.Models
{
    public class DirectorRemoval
    {
        private readonly DblibraryContext _context;
        private readonly Director _director;

        public DirectorRemoval(DblibraryContext context, Director director)
        {
            _context = context;
            _director = director;
        }

        public void DeleteDirector()
        {
            var librariesWithDirector = _context.Libraries
                .Where(l => l.DirectorId == _director.Id)
                .ToList();

            foreach (var library in librariesWithDirector)
            {
                library.DirectorId = null;  
            }

            _context.Directors.Remove(_director);

            _context.SaveChanges();
        }
    }
}
