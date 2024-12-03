using Microsoft.EntityFrameworkCore;
using Bookstore.Models;
using NuGet.Protocol.Plugins;

namespace Bookstore.Data
{
    public class BookstoreContext : DbContext
    {
        public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options)
        {
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Seller> Seller { get; set; }  
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Book> Books { get; set; }
	}

}