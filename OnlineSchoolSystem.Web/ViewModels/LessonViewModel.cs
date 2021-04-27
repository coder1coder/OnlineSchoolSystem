using System.ComponentModel.DataAnnotations;

namespace OnlineSchoolSystem.Web.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Заголовок")]
        public string Title { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
