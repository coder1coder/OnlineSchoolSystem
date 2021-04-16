using Microsoft.EntityFrameworkCore;
using OnlineSchoolSystem.Domain.Models;

namespace OnlineSchoolSystem.DataAccess.EFCore
{
    public class ApplicationContext: DbContext
    {
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Member> Members { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }

    
}
