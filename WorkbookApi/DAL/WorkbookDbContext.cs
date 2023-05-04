using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WorkbookApi.DAL.Entities;

namespace WorkbookApi.DAL
{
    public class WorkbookDbContext : DbContext
    {
        public WorkbookDbContext(DbContextOptions<WorkbookDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.Id);

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }

}
