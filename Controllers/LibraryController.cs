using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using LR4_ASP_net.Models;

namespace LR4_ASP_net.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public LibraryController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
        }

        [HttpGet("Library")]
        public IActionResult Index()
        {
            return Content("Ласкаво просимо до бібліотеки!", "text/plain", System.Text.Encoding.UTF8);
        }

        [HttpGet("Library/Books")]
        public IActionResult GetAllBooks()
        {
            var books = _configuration.GetSection("Books").Get<Dictionary<string, BookInfoModel>>();
            if (books == null || !books.Any())
            {
                return NotFound("Книги не знайдено.");
            }
            return Json(books, _jsonOptions);
        }

        [HttpGet("Library/Books/{id}")]
        public IActionResult GetBookById(string id)
        {
            var books = _configuration.GetSection("Books").Get<Dictionary<string, BookInfoModel>>();
            if (books == null || !books.Any())
            {
                return NotFound("Книги не знайдено.");
            }

            if (books.TryGetValue(id, out var book))
            {
                return Json(book, _jsonOptions);
            }

            return NotFound($"Книгу з ID {id} не знайдено.");
        }

        [HttpGet("Library/Profile/{id?}")]
        public IActionResult Profile(int? id)
        {
            var profiles = _configuration.GetSection("Profiles").Get<Dictionary<string, Dictionary<string, object>>>();

            if (!id.HasValue)
            {
                if (profiles.TryGetValue("DefaultUser", out var defaultProfile))
                {
                    return Json(defaultProfile, _jsonOptions);
                }
                return NotFound("Профіль гостя не знайдено.");
            }

            if (id < 0 || id > 5)
            {
                return BadRequest("Неправильний ID. Будь ласка, введіть ID від 0 до 5.");
            }

            string profileKey = id.ToString();

            if (profiles.TryGetValue(profileKey, out var profile))
            {
                return Json(profile, _jsonOptions);
            }

            return NotFound("Профіль не знайдено.");
        }
    }
}