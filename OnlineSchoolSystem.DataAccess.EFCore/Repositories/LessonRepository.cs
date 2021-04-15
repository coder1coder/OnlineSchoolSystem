using OnlineSchoolSystem.Domain.Interfaces;
using OnlineSchoolSystem.Domain.Models;

namespace OnlineSchoolSystem.DataAccess.EFCore.Repositories
{
    public class LessonRepository: GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(ApplicationContext context):base(context)
        {
        }
    }
}
