using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wedding.ef;
using Wedding.ef.Entities;

namespace Wedding.Controllers
{
    public class ImportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invitations = await _context.Invitations.Include(a => a.Attendees).ToListAsync();
            return View(invitations);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string csv)
        {
            var attendees = await _context.Attendees.ToListAsync();

            var splitByLine = csv.Split(Environment.NewLine);

            foreach (string line in splitByLine)
            {
                var splitByComma = line.Split(',');

                Invitation invitation = new Invitation()
                {
                    PublicId = Guid.NewGuid(),
                    SendTo = splitByComma[4],
                    Attendees = new List<Attendee>()
                };

                if (!string.IsNullOrEmpty(splitByComma[0]))
                {
                    invitation.Attendees.Add(new Attendee() { FirstName = splitByComma[0], LastName = splitByComma[1] });
                }

                if (!string.IsNullOrEmpty(splitByComma[2]))
                {
                    invitation.Attendees.Add(new Attendee() { FirstName = splitByComma[2], LastName = splitByComma[3] });
                }

                await _context.AddAsync(invitation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
