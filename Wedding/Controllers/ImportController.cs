using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
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

        public async Task<IActionResult> SendEmails()
        {
            var inviations = await _context.Invitations
                    .Where(a => a.SentTimeStamp == null)
                    .Take(100)
                    .ToListAsync();

            if (inviations.Count > 0)
            {
                // TODO setup a sendgrid account and put in the api here
                // They also say they do something with templates, but it might be a paid feature.
                // Dunno just do plain text for now.
                var apiKey = System.Configuration.ConfigurationManager.AppSettings["SendGridApiKey"];
                var client = new SendGridClient(apiKey);

                foreach (var invitation in inviations)
                {
                    var from = new EmailAddress("ciaraandianwedding@gmail.com", "Ciara and Ian");
                    var subject = "Wedding Invitation";
                    var to = new EmailAddress(invitation.SendTo);
                    var plainTextContent = "This is a test email";
                    var htmlContent = "<strong>with some html</strong>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    var response = await client.SendEmailAsync(msg);

                    invitation.SentTimeStamp = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
