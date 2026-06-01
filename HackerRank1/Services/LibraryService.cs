using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryService.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.WebAPI.Services
{
    public class LibrariesService : ILibrariesService
    {
        private readonly LibraryContext _libraryContext;

        public LibrariesService(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<IEnumerable<Library>> Get(int[] ids)
        {
            var libraries = _libraryContext.Libraries.AsQueryable();

            if (ids != null && ids.Any())
            {
                libraries = libraries.Where(x => ids.Contains(x.Id));
            }

            return await libraries.ToListAsync();
        }

        public async Task<Library> Add(Library library)
        {
            await _libraryContext.Libraries.AddAsync(library);
            await _libraryContext.SaveChangesAsync();

            return library;
        }

        public async Task<IEnumerable<Library>> AddRange(IEnumerable<Library> libraries)
        {
            await _libraryContext.Libraries.AddRangeAsync(libraries);
            await _libraryContext.SaveChangesAsync();

            return libraries;
        }

        public async Task<Library> Update(Library library)
        {
            var libraryForChanges = await _libraryContext.Libraries
                .SingleAsync(x => x.Id == library.Id);

            libraryForChanges.Name = library.Name;
            libraryForChanges.Location = library.Location;

            _libraryContext.Libraries.Update(libraryForChanges);
            await _libraryContext.SaveChangesAsync();

            return libraryForChanges;
        }

        public async Task<bool> Delete(Library library)
        {
            var libraryToDelete = await _libraryContext.Libraries
                .FirstOrDefaultAsync(x => x.Id == library.Id);

            if (libraryToDelete == null)
            {
                return false;
            }

            var books = await _libraryContext.Books
                .Where(x => x.LibraryId == library.Id)
                .ToListAsync();

            if (books.Any())
            {
                _libraryContext.Books.RemoveRange(books);
            }

            _libraryContext.Libraries.Remove(libraryToDelete);

            await _libraryContext.SaveChangesAsync();

            return true;
        }
    }

    public interface ILibrariesService
    {
        Task<IEnumerable<Library>> Get(int[] ids);

        Task<Library> Add(Library library);

        Task<IEnumerable<Library>> AddRange(IEnumerable<Library> libraries);

        Task<Library> Update(Library library);

        Task<bool> Delete(Library library);
    }
}