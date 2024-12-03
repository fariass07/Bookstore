using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
	public class BookService
	{
		private readonly BookstoreContext _context;

		public BookService(BookstoreContext context)
		{
			_context = context;
		}

        public async Task<List<Book>> FindAllAsync()
        {
            return await _context.Books.Include(x => x.Genres).ToListAsync();
        }

        public async Task InsertAsync(Book book)
		{
			_context.Add(book);
			await _context.SaveChangesAsync();
		}

        public async Task<Book> FindByIdAsync(int id)
        {
            return await _context.Books.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(BookFormViewModel viewmodel)
        {
            bool hasAny = await _context.Books.AnyAsync(x => x.Id == viewmodel.Book.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado");
            }

            try
            {
                Book? dbBook = await _context.Books.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == viewmodel.Book.Id);

                List<Genre> selectedGenres = new List<Genre>();

                foreach (int genreId in viewmodel.SelectedGenresIds)
                {
                    Genre genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == genreId);
                    if (genre is not null)
                    {
                        selectedGenres.Add(genre);
                    }
                }
                List<Genre> currentGenres = dbBook.Genres.ToList();

                List<Genre> genresToRemove = currentGenres.Where(current => !selectedGenres.Any(selected => selected.Id == current.Id)).ToList();

                List<Genre> genresToAdd = selectedGenres.Where(selected => !currentGenres.Any(current => current.Id == selected.Id)).ToList();

                foreach (Genre genre in genresToRemove)
                {
                    dbBook.Genres.Remove(genre);
                }

                foreach (Genre genre in genresToAdd)
                {
                    dbBook.Genres.Add(genre);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbConcorrencyException(ex.Message);
            }
        }
        public async Task RemoveAsync(int id)
		{
			try
			{
				var obj = await _context.Books.FindAsync(id);
				_context.Books.Remove(obj);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new IntegrityException(ex.Message);
			}
		}

	}
}
