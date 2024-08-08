using System;
using System.Collections.Generic;
using LibraryDomain.Model;
using LibraryInfrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryInfrastructure;

public partial class DblibraryContext : DbContext
{
    public DblibraryContext()
    {
    }

    public DblibraryContext(DbContextOptions<DblibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Director> Directors { get; set; }

    public virtual DbSet<Library> Libraries { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<LibraryDomain.Model.Type> Types { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HOME-PC\\SQLEXPRESS; Database=DBLibrary; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasMany(d => d.Books).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorsBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_AuthorsBooks_Books"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_AuthorsBooks_Authors"),
                    j =>
                    {
                        j.HasKey("AuthorId", "BookId");
                        j.ToTable("AuthorsBooks");
                    });
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Description).HasColumnType("nvarchar(MAX)");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Books_Publishers");
        });

        modelBuilder.Entity<Director>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Gender).WithMany(p => p.Directors)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Directors_Genders");
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Director).WithMany(p => p.Libraries)
                .HasForeignKey(d => d.DirectorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Libraries_Directors");

            entity.HasOne(d => d.Type).WithMany(p => p.Libraries)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Libraries_Types");

            entity.HasMany(d => d.Books).WithMany(p => p.Libraries)
                .UsingEntity<Dictionary<string, object>>(
                    "LibrariesBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_LibrariesBooks_Books"),
                    l => l.HasOne<Library>().WithMany()
                        .HasForeignKey("LibraryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_LibrariesBooks_Libraries"),
                    j =>
                    {
                        j.HasKey("LibraryId", "BookId");
                        j.ToTable("LibrariesBooks");
                    });
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Info).HasColumnType("nvarchar(MAX)");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<LibraryDomain.Model.Type>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
