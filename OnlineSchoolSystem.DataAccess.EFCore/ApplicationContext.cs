using Microsoft.EntityFrameworkCore;
using OnlineSchoolSystem.Domain.Models;

namespace OnlineSchoolSystem.DataAccess.EFCore
{
    public class ApplicationContext: DbContext
    {
        public DbSet<Lesson> Lessons { get; set; }
    }
}
