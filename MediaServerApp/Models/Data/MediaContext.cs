using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MediaServerApp.Models.Data;

public partial class MediaContext : DbContext
{
    public MediaContext()
    {
    }

    public MediaContext(DbContextOptions<MediaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<SavedItem> SavedItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WatchHistory> WatchHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Genres_pkey");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Movies_pkey");

            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("MovieGenres_GenreId_fkey"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .HasConstraintName("MovieGenres_MovieId_fkey"),
                    j =>
                    {
                        j.HasKey("MovieId", "GenreId").HasName("MovieGenres_pkey");
                        j.ToTable("MovieGenres");
                    });
        });

        modelBuilder.Entity<SavedItem>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MovieId }).HasName("SavedItems_pkey");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Movie).WithMany(p => p.SavedItems)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("SavedItems_MovieId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SavedItems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("SavedItems_UserId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<WatchHistory>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MovieId }).HasName("WatchHistory_pkey");

            entity.ToTable("WatchHistory");

            entity.Property(e => e.LastWatched).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Movie).WithMany(p => p.WatchHistories)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("WatchHistory_MovieId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.WatchHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("WatchHistory_UserId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
