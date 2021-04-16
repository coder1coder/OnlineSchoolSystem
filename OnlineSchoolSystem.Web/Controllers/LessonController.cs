using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSchoolSystem.Domain.Interfaces;
using OnlineSchoolSystem.Web.Models;
using System.Diagnostics;
using System.Linq;

namespace OnlineSchoolSystem.Web.Controllers
{
    public class LessonController : Controller
    {
        private readonly ILogger<LessonController> _logger;
        private readonly ILessonRepository _repository;

        public LessonController(ILogger<LessonController> logger, ILessonRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult Index()
        {
            var model = _repository.GetAll().ToList();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
