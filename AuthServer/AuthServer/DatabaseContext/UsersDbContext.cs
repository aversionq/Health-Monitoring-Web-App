using System;
using System.Collections.Generic;
using AuthServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AuthServer.DatabaseContext
{
    public partial class UsersDbContext : DbContext
    {
        public UsersDbContext()
        {
        }

        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public virtual DbSet<DoctorPatient> DoctorPatients { get; set; } = null!;
        public virtual DbSet<DoctorRequest> DoctorRequests { get; set; } = null!;
        public virtual DbSet<UserChat> UserChats { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName).HasMaxLength(70);

                entity.Property(e => e.Gender).HasDefaultValueSql("((0))");

                entity.Property(e => e.Height).HasDefaultValueSql("((0.0000000000000000e+000))");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPAddress")
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.LastName).HasMaxLength(70);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.Weight).HasDefaultValueSql("((0.0000000000000000e+000))");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FromUser).HasMaxLength(450);

                entity.Property(e => e.LastMessageDate).HasPrecision(0);

                entity.Property(e => e.LastMessageText).HasMaxLength(400);

                entity.HasOne(d => d.FromUserNavigation)
                    .WithMany(p => p.Chats)
                    .HasForeignKey(d => d.FromUser)
                    .HasConstraintName("FK_FromUserChat");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.FromUser).HasMaxLength(450);

                entity.Property(e => e.MessageText).HasMaxLength(500);

                entity.Property(e => e.SentAt).HasPrecision(0);

                entity.Property(e => e.ToUser).HasMaxLength(450);

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.ChatId)
                    .HasConstraintName("FK_MessageChat");

                entity.HasOne(d => d.FromUserNavigation)
                    .WithMany(p => p.ChatMessageFromUserNavigations)
                    .HasForeignKey(d => d.FromUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FromUserMessage");

                entity.HasOne(d => d.ToUserNavigation)
                    .WithMany(p => p.ChatMessageToUserNavigations)
                    .HasForeignKey(d => d.ToUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ToUserMessage");
            });

            modelBuilder.Entity<DoctorPatient>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DoctorId).HasMaxLength(450);

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.DoctorPatientDoctors)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DoctorUser");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DoctorPatientUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientUser");
            });

            modelBuilder.Entity<DoctorRequest>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("DoctorRequests_PK");

                entity.Property(e => e.DiplomaPicture).HasMaxLength(450);

                entity.Property(e => e.PassportPicture).HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.DoctorRequest)
                    .HasForeignKey<DoctorRequest>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserDoctor");
            });

            modelBuilder.Entity<UserChat>(entity =>
            {
                entity.HasIndex(e => new { e.ChatId, e.UserId }, "UserChats_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.UserChats)
                    .HasForeignKey(d => d.ChatId)
                    .HasConstraintName("FK_Chat");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserChats)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
