using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OyunKedisi.Models;

public partial class OyunKedisiDbContext : DbContext
{
    public OyunKedisiDbContext()
    {
    }

    public OyunKedisiDbContext(DbContextOptions<OyunKedisiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Favori> Favoris { get; set; }

    public virtual DbSet<Oyunlar> Oyunlars { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Yetki> Yetkis { get; set; }

    public virtual DbSet<Yorumlar> Yorumlars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-B19C3D2;Initial Catalog=OyunKedisiDB;Integrated Security =True;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favori>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favori__3214EC2742798D48");

            entity.ToTable("Favori");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.HasOne(d => d.Oyun).WithMany(p => p.Favoris)
                .HasForeignKey(d => d.OyunId)
                .HasConstraintName("FK__Favori__OyunId__59FA5E80");

            entity.HasOne(d => d.User).WithMany(p => p.Favoris)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Favori__UserId__59063A47");
        });

        modelBuilder.Entity<Oyunlar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Oyunlar__3214EC27829A8B05");

            entity.ToTable("Oyunlar");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EklemeTarihi).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.OyunAdi).HasMaxLength(100);
            entity.Property(e => e.OyunLinki).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Oyunlars)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Oyunlar__UserId__5629CD9C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27AA08D005");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.KayitTarihi).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
            entity.Property(e => e.Mail).HasMaxLength(100);
            entity.Property(e => e.SifreHash).HasMaxLength(200);

            entity.HasOne(d => d.Yetki).WithMany(p => p.Users)
                .HasForeignKey(d => d.YetkiId)
                .HasConstraintName("FK__Users__YetkiId__534D60F1");
        });

        modelBuilder.Entity<Yetki>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Yetki__3214EC270FE19EEE");

            entity.ToTable("Yetki");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.YetkiAdi).HasMaxLength(50);
        });

        modelBuilder.Entity<Yorumlar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Yorumlar__3214EC2775F53FDD");

            entity.ToTable("Yorumlar");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Yorumlar1).HasColumnName("Yorumlar");

            entity.HasOne(d => d.Oyun).WithMany(p => p.Yorumlars)
                .HasForeignKey(d => d.OyunId)
                .HasConstraintName("FK__Yorumlar__OyunId__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.Yorumlars)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Yorumlar__UserId__5CD6CB2B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
