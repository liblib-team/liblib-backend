using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace liblib_backend.Models
{
    public partial class MyLibContext : DbContext
    {
        public MyLibContext()
        {
        }

        public MyLibContext(DbContextOptions<MyLibContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookAuthor> BookAuthor { get; set; }
        public virtual DbSet<BookLending> BookLending { get; set; }
        public virtual DbSet<BookReservation> BookReservation { get; set; }
        public virtual DbSet<BookSubject> BookSubject { get; set; }
        public virtual DbSet<Ebook> Ebook { get; set; }
        public virtual DbSet<Hardbook> Hardbook { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Publisher> Publisher { get; set; }
        public virtual DbSet<Rating> Rating { get; set; }
        public virtual DbSet<Subject> Subject { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Image).IsRequired();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Image).IsRequired();

                entity.Property(e => e.Language)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Publisher)
                    .WithMany(p => p.Book)
                    .HasForeignKey(d => d.PublisherId)
                    .HasConstraintName("FK_Book_Publisher");
            });

            modelBuilder.Entity<BookAuthor>(entity =>
            {
                entity.HasKey(e => new { e.BookId, e.AuthorId })
                    .HasName("PK_Book_Author_1");

                entity.ToTable("Book_Author");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.BookAuthor)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Book_Author_Author");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookAuthor)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Book_Author_Book");
            });

            modelBuilder.Entity<BookLending>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Hardbook)
                    .WithMany(p => p.BookLending)
                    .HasForeignKey(d => d.HardbookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookLending_Hardbook");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.BookLending)
                    .HasForeignKey<BookLending>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookLending_BookReservation");
            });

            modelBuilder.Entity<BookReservation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.BookReservation)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookReservation_Account");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookReservation)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookReservation_Book");
            });

            modelBuilder.Entity<BookSubject>(entity =>
            {
                entity.HasKey(e => new { e.BookId, e.SubjectId });

                entity.ToTable("Book_Subject");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookSubject)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Book_Subject_Book");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.BookSubject)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Book_Subject_Subject");
            });

            modelBuilder.Entity<Ebook>(entity =>
            {
                entity.HasKey(e => e.BookId);

                entity.Property(e => e.BookId).ValueGeneratedNever();

                entity.Property(e => e.Location).IsRequired();

                entity.HasOne(d => d.Book)
                    .WithOne(p => p.Ebook)
                    .HasForeignKey<Ebook>(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ebook_Book");
            });

            modelBuilder.Entity<Hardbook>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Barcode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Hardbook)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hardbook_Book");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Action).IsRequired();

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Log_Account");
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.BookId });

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Rating)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_Account");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Rating)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_Book");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsRequired();
            });
        }
    }
}
