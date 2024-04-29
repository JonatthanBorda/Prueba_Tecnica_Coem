using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Prueba_Tecnica_Coem.Models;

public partial class DbPortalCoemContext : IdentityDbContext<IdentityUser>
{
    public DbPortalCoemContext()
    {
    }

    public DbPortalCoemContext(DbContextOptions<DbPortalCoemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aplicaciones> Aplicaciones { get; set; }

    public virtual DbSet<Demandantes> Demandantes { get; set; }

    public virtual DbSet<Empleadores> Empleadores { get; set; }

    public virtual DbSet<EstadoAplicacion> EstadoAplicacions { get; set; }

    public virtual DbSet<NivelEducativo> NivelEducativos { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<Vacantes> Vacantes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlServer("Server=Jonathan_Borda; Database=db_Portal_Coem; Integrated security=true; Encrypt=False");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Aplicaciones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Aplicaci__3214EC076EFB285F");

            entity.Property(e => e.FechaAplicacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdDemandanteNavigation).WithMany(p => p.Aplicaciones)
                .HasForeignKey(d => d.IdDemandante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Aplicacio__IdDem__38996AB5");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Aplicaciones)
                .HasForeignKey(d => d.IdEstado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Aplicacio__IdEst__398D8EEE");

            entity.HasOne(d => d.IdVacanteNavigation).WithMany(p => p.Aplicaciones)
                .HasForeignKey(d => d.IdVacante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Aplicacio__IdVac__37A5467C");
        });

        modelBuilder.Entity<Demandantes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Demandan__3214EC07CB41699E");

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Celular).HasMaxLength(10);
            entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
            entity.Property(e => e.Nombres).HasMaxLength(100);

            entity.HasOne(d => d.IdNivelEducativoNavigation).WithMany(p => p.Demandantes)
                .HasForeignKey(d => d.IdNivelEducativo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Demandant__IdNiv__2C3393D0");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Demandantes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Demandant__IdUsu__2B3F6F97");
        });

        modelBuilder.Entity<Empleadores>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empleado__3214EC0739502845");

            entity.Property(e => e.Industria).HasMaxLength(100);
            entity.Property(e => e.Localizacion).HasMaxLength(255);
            entity.Property(e => e.RazonSocial).HasMaxLength(255);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Empleadores)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Empleador__IdUsu__2F10007B");
        });

        modelBuilder.Entity<EstadoAplicacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EstadoAp__3214EC073C4A4A52");

            entity.ToTable("EstadoAplicacion");

            entity.Property(e => e.Estado).HasMaxLength(255);
        });

        modelBuilder.Entity<NivelEducativo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NivelEdu__3214EC0785C0F89B");

            entity.ToTable("NivelEducativo");

            entity.Property(e => e.Nivel).HasMaxLength(255);
        });

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TipoUsua__3214EC075EC6FAE8");

            entity.ToTable("TipoUsuario");

            entity.Property(e => e.Tipo).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC072CC3DD88");

            entity.Property(e => e.Clave).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);

            entity.HasOne(d => d.IdTipoUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdTipoUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__IdTipo__267ABA7A");
        });

        modelBuilder.Entity<Vacantes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vacantes__3214EC07702C3CC1");

            entity.HasOne(d => d.IdEmpleadorNavigation).WithMany(p => p.Vacantes)
                .HasForeignKey(d => d.IdEmpleador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vacantes__IdEmpl__31EC6D26");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
