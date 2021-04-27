using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSchoolSystem.Domain.Interfaces;
using OnlineSchoolSystem.Domain.Models;
using OnlineSchoolSystem.Web.Models;
using OnlineSchoolSystem.Web.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSchoolSystem.Web.Controllers
{
    public class LessonsController : Controller
    {
        private readonly ILogger<LessonsController> _logger;
        private readonly ILessonRepository _lessons;

        public LessonsController(ILogger<LessonsController> logger, ILessonRepository repository)
        {
            _logger = logger;
            _lessons = repository;
        }

        public IActionResult Index()
        {
            var model = _lessons.GetAll().ToList();
            return View(model);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(LessonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(x => x.CreateMap<LessonViewModel, Lesson>());
                var mapper = new Mapper(config);

                var lesson = mapper.Map<LessonViewModel, Lesson>(model);
                
                _lessons.Add(lesson);

                await _lessons.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
