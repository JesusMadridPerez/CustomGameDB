using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CustomGameDB.Models;

public partial class NeondbContext : DbContext
{
    public NeondbContext()
    {
    }

    public NeondbContext(DbContextOptions<NeondbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amistade> Amistades { get; set; }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Plataform> Plataforms { get; set; }

    public virtual DbSet<Plataform1> Plataforms1 { get; set; }

    public virtual DbSet<ReviewsUsuario> ReviewsUsuarios { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ep-soft-dream-ab77jchm-pooler.eu-west-2.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_ZkR4sjiExSh2;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amistade>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario1, e.IdUsuario2 }).HasName("amistades_pkey");

            entity.ToTable("amistades");

            entity.Property(e => e.IdUsuario1).HasColumnName("id_usuario_1");
            entity.Property(e => e.IdUsuario2).HasColumnName("id_usuario_2");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'aceptada'::character varying")
                .HasColumnName("estado").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
            entity.Property(e => e.FechaAmistad)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_amistad");

            entity.HasOne(d => d.IdUsuario1Navigation).WithMany(p => p.AmistadeIdUsuario1Navigations)
                .HasForeignKey(d => d.IdUsuario1)
                .HasConstraintName("fk_usuario_1");

            entity.HasOne(d => d.IdUsuario2Navigation).WithMany(p => p.AmistadeIdUsuario2Navigations)
                .HasForeignKey(d => d.IdUsuario2)
                .HasConstraintName("fk_usuario_2");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Iddeveloper).HasName("developers_pkey");

            entity.ToTable("developers");

            entity.Property(e => e.Iddeveloper)
                .ValueGeneratedNever()
                .HasColumnName("iddeveloper");
            entity.Property(e => e.Valuedeveloper)
                .HasMaxLength(255)
                .HasColumnName("valuedeveloper");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_pkey");

            entity.ToTable("games");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BackgroundImage)
                .HasMaxLength(255)
                .HasColumnName("background_image").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
            entity.Property(e => e.Metacritic).HasColumnName("metacritic");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("_name").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
            entity.Property(e => e.Notapersonal).HasColumnName("notapersonal");
            entity.Property(e => e.Playtime).HasColumnName("playtime");
            entity.Property(e => e.Raiting).HasColumnName("raiting");
            entity.Property(e => e.RatingTop).HasColumnName("rating_top");
            entity.Property(e => e.RatingsCount).HasColumnName("ratings_count");
            entity.Property(e => e.Released).HasColumnName("released");
            entity.Property(e => e.Reviewpersonal).HasColumnName("reviewpersonal");
            entity.Property(e => e.ReviewsTextCount).HasColumnName("reviews_text_count");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.Tba).HasColumnName("tba");

            entity.HasMany(d => d.Iddevelopers).WithMany(p => p.Idgames)
                .UsingEntity<Dictionary<string, object>>(
                    "Developersgame",
                    r => r.HasOne<Developer>().WithMany()
                        .HasForeignKey("Iddeveloper")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_iddeveloper"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("Idgame")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_idgamedeveloper"),
                    j =>
                    {
                        j.HasKey("Idgame", "Iddeveloper").HasName("developersgame_pkey");
                        j.ToTable("developersgame");
                        j.IndexerProperty<int>("Idgame").HasColumnName("idgame");
                        j.IndexerProperty<int>("Iddeveloper").HasColumnName("iddeveloper");
                    });

            entity.HasMany(d => d.Idstores).WithMany(p => p.Idgames)
                .UsingEntity<Dictionary<string, object>>(
                    "Storesgame",
                    r => r.HasOne<Store>().WithMany()
                        .HasForeignKey("Idstore")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_idstore"),
                    l => l.HasOne<Game>().WithMany()
                        .HasForeignKey("Idgame")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_idgamestore"),
                    j =>
                    {
                        j.HasKey("Idgame", "Idstore").HasName("storesgames_pkey");
                        j.ToTable("storesgames");
                        j.IndexerProperty<int>("Idgame").HasColumnName("idgame");
                        j.IndexerProperty<int>("Idstore").HasColumnName("idstore");
                    });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.IdGenres).HasName("genres_pkey");

            entity.ToTable("genres");

            entity.Property(e => e.IdGenres)
                .ValueGeneratedNever()
                .HasColumnName("id_genres");
            entity.Property(e => e.ValueGenres)
                .HasMaxLength(255)
                .HasColumnName("value_genres").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );

            entity.HasMany(d => d.IdGames).WithMany(p => p.IdGenres)
                .UsingEntity<Dictionary<string, object>>(
                    "Genresgame",
                    r => r.HasOne<Game>().WithMany()
                        .HasForeignKey("IdGame")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_id_game"),
                    l => l.HasOne<Genre>().WithMany()
                        .HasForeignKey("IdGenres")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_id_genres"),
                    j =>
                    {
                        j.HasKey("IdGenres", "IdGame").HasName("genresgames_pkey");
                        j.ToTable("genresgames");
                        j.IndexerProperty<int>("IdGenres").HasColumnName("id_genres");
                        j.IndexerProperty<int>("IdGame").HasColumnName("id_game");
                    });
        });

        modelBuilder.Entity<Plataform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("plataform_pkey");

            entity.ToTable("plataform");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("_name").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<Plataform1>(entity =>
        {
            entity.HasKey(e => new { e.Idgame, e.Idplataform }).HasName("plataforms_pkey");

            entity.ToTable("plataforms");

            entity.Property(e => e.Idgame).HasColumnName("idgame");
            entity.Property(e => e.Idplataform).HasColumnName("idplataform");
            entity.Property(e => e.ReleasedAt)
                .HasMaxLength(255)
                .HasColumnName("released_at");
            entity.Property(e => e.RequirementMaximun).HasColumnName("requirement_maximun");
            entity.Property(e => e.RequirementMinimun).HasColumnName("requirement_minimun");

            entity.HasOne(d => d.IdgameNavigation).WithMany(p => p.Plataform1s)
                .HasForeignKey(d => d.Idgame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_idgame");

            entity.HasOne(d => d.IdplataformNavigation).WithMany(p => p.Plataform1s)
                .HasForeignKey(d => d.Idplataform)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_idplataform");
        });

        modelBuilder.Entity<ReviewsUsuario>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdGame }).HasName("reviews_usuarios_pkey");

            entity.ToTable("reviews_usuarios");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Esfavorito).HasColumnName("esfavorito");
            entity.Property(e => e.Estadojuego)
                .HasMaxLength(100)
                .HasColumnName("estadojuego");
            entity.Property(e => e.FechaUltimaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_ultima_modificacion");
            entity.Property(e => e.HorasJugadas).HasColumnName("horas_jugadas");
            entity.Property(e => e.NotaPersonal)
                .HasPrecision(3, 1)
                .HasColumnName("nota_personal");
            entity.Property(e => e.ReviewTexto).HasColumnName("review_texto").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
            entity.Property(e => e.rutaJuego).HasColumnName("rutaJuego").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.ReviewsUsuarios)
                .HasForeignKey(d => d.IdGame)
                .HasConstraintName("fk_review_game");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ReviewsUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("fk_review_usuario");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Idstore).HasName("stores_pkey");

            entity.ToTable("stores");

            entity.Property(e => e.Idstore)
                .ValueGeneratedNever()
                .HasColumnName("idstore");
            entity.Property(e => e.Valuestore)
                .HasMaxLength(255)
                .HasColumnName("valuestore").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Idusuario).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.Property(e => e.Idusuario)
                .ValueGeneratedNever()
                .HasColumnName("idusuario");
            entity.Property(e => e.Anyonacimiento).HasColumnName("anyonacimiento");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .HasColumnName("user_password").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username").HasConversion(
            v => encriptacion.Encriptacion.Encrypt(v),
            v => encriptacion.Encriptacion.Decrypt(v)
        );
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
