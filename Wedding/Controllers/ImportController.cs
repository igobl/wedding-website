using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using Wedding.ef;
using Wedding.ef.Entities;

namespace Wedding.Controllers
{
    public class ImportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ImportController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                    .Include(a => a.Attendees)
                    .Where(a => a.SentTimeStamp == null)
                    .Take(100)
                    .ToListAsync();

            if (inviations.Count > 0)
            {
                // TODO setup a sendgrid account and put in the api here
                // They also say they do something with templates, but it might be a paid feature.
                // Dunno just do plain text for now.
                var apiKey = _configuration.GetValue<string>("SendGridApiKey");
                var invitationBaseUrl = _configuration.GetValue<string>("InvitationBaseUrl");
                var client = new SendGridClient(apiKey);

                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "templates", "invite.html");
                string templateContent = await System.IO.File.ReadAllTextAsync(templatePath);
                
                foreach (var invitation in inviations)
                {
                    string guestFirstNames = string.Join("&", invitation.Attendees.Select(a => a.FirstName));
                    var from = new EmailAddress("invite@ciaraandianwedding.ie", "Ciara and Ian");
                    var subject = "Invitation - Ciara & Ian's Wedding";
                    var to = new EmailAddress(invitation.SendTo);
                    var htmlContent = templateContent.
                        Replace("{guest_name_placeholder}", guestFirstNames)
                        .Replace("{rsvp_link_placeholder}", $"{invitationBaseUrl}{invitation.PublicId}");
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
                    var response = await client.SendEmailAsync(msg);
                    var responseBody = await response.Body.ReadAsStringAsync();

                    Console.WriteLine(responseBody);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(responseBody);
                    }

                    invitation.SentTimeStamp = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
