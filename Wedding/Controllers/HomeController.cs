using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Wedding.ef;
using Wedding.ef.Entities;
using Wedding.Models;
using Rsvp = Wedding.Models.Rsvp;

namespace Wedding.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string id)
        {
            Invitation? invitation = null;
            if (Guid.TryParse(id, out Guid guid))
            {
                invitation = _context.Invitations
                    .Include(a => a.Attendees)
                    .Include(a => a.Rsvp)
                    .FirstOrDefault(a => a.PublicId == guid);
            }
            return View(invitation);
        }

        [HttpPost]
        public async Task<IActionResult> Rsvp(string rsvpid, string rsvp, string notes)
        {
            if(Guid.TryParse(rsvpid, out Guid publicId) && bool.TryParse(rsvp, out bool isAttending))
            {
                var invitation = await _context.Invitations.FirstAsync(a => a.PublicId == publicId);

                var rsvpResponse = new ef.Entities.Rsvp()
                {
                    IsAttending = isAttending,
                    Notes = notes,
                    InvitationId = invitation.Id,
                    Timestamp = DateTime.UtcNow
                };

                await _context.Rsvps.AddAsync(rsvpResponse);
                await _context.SaveChangesAsync();

                return View("RsvpConfirmation", rsvpResponse);
            }

            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Rsvp()
        {
                return View("RsvpConfirmation", new ef.Entities.Rsvp());
        }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}