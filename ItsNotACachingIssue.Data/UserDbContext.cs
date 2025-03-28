using Microsoft.EntityFrameworkCore;

namespace ItsNotACachingIssue.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> opts) : DbContext(opts)
    {
        public UserDbContext() : this(new DbContextOptions<UserDbContext>())
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");

                e.HasKey("UserId");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
