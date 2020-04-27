using Microsoft.EntityFrameworkCore;

namespace belicious.Models.Persistence
{
    public class BookmarksContext : DbContext
    {
        public BookmarksContext(DbContextOptions<BookmarksContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TagBookmark>().HasKey(table => new {
                table.tagId, table.bookmarkId
            });
            builder.Entity<UserBookmark>().HasKey(table => new {
                table.userId, table.bookmarkId
            });
        }

        public DbSet<Bookmark> Bookmarks {get; set;}
        public DbSet<UserBookmark> UserBookmarks {get; set;}

        public DbSet<Tag> Tags {get; set;}

        public DbSet<TagBookmark> TagBookmarks {get; set;}
    }
}