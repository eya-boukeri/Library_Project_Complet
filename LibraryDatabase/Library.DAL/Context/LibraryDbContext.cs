using Library.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.DAL.Context
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book configuration
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(300);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(200);
                entity.Property(b => b.ISBN).HasMaxLength(20);
                entity.HasIndex(b => b.ISBN).IsUnique();
            });

            // Borrowing configuration
            modelBuilder.Entity<Borrowing>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.HasOne(b => b.Book)
                      .WithMany(bk => bk.Borrowings)
                      .HasForeignKey(b => b.BookId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Le Petit Prince", Author = "Antoine de Saint-Exupéry", ISBN = "978-2-07-040850-4", Genre = "Littérature", PublicationYear = 1943, Publisher = "Gallimard", Description = "Un conte poétique et philosophique qui traite de la solitude, de l'amitié et de l'amour.", IsAvailable = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Book { Id = 2, Title = "Les Misérables", Author = "Victor Hugo", ISBN = "978-2-07-040953-2", Genre = "Roman historique", PublicationYear = 1862, Publisher = "A. Lacroix", Description = "Un roman historique sur la vie de Jean Valjean, ancien forçat, dans la France du XIXe siècle.", IsAvailable = true, CreatedAt = new DateTime(2024, 1, 1) },
                new Book { Id = 3, Title = "L'Étranger", Author = "Albert Camus", ISBN = "978-2-07-036024-5", Genre = "Roman philosophique", PublicationYear = 1942, Publisher = "Gallimard", Description = "L'histoire de Meursault, un pied-noir d'Algérie indifférent à tout.", IsAvailable = false, CreatedAt = new DateTime(2024, 1, 1) },
                new Book { Id = 4, Title = "Madame Bovary", Author = "Gustave Flaubert", ISBN = "978-2-07-036024-9", Genre = "Roman réaliste", PublicationYear = 1857, Publisher = "Revue de Paris", Description = "L'histoire d'Emma Bovary, femme d'un médecin de campagne, qui rêve d'une vie meilleure.", IsAvailable = true, CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}
