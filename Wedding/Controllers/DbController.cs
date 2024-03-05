using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wedding.ef;

namespace Wedding.Controllers
{
    public class DbController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DbController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Update()
        {
            _context.Database.Migrate();
            return Json("Migrations Complete");
        }
    }
}
