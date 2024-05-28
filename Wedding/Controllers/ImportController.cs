using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
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
        public async Task<IActionResult> Index(string id)
        {
            if (id != _configuration.GetValue<string>("ImportSecretKey"))
            {
                return Unauthorized();
            }

            var invitations = await _context.Invitations
                .Include(a => a.Attendees)
                .Include(a => a.Rsvp)
                .ToListAsync();
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
                    invitation.Attendees.Add(new Attendee()
                    { FirstName = splitByComma[0], LastName = splitByComma[1] });
                }

                if (!string.IsNullOrEmpty(splitByComma[2]))
                {
                    invitation.Attendees.Add(new Attendee()
                    { FirstName = splitByComma[2], LastName = splitByComma[3] });
                }

                await _context.AddAsync(invitation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SendEmails()
        {
            var inviations = await _context.Invitations
                .Include(a => a.Attendees)
                .Where(a => a.SentTimeStamp == null)
                .Take(100)
                .ToListAsync();

            if (inviations.Count > 0)
            {
                var invitationBaseUrl = _configuration.GetValue<string>("InvitationBaseUrl");
                MailjetClient client = new MailjetClient(
                    _configuration.GetValue<string>("MailJetApiKey"),
                    _configuration.GetValue<string>("MailJetApiSecret"));

                foreach (var invitation in inviations)
                {
                    string guestFirstNames = string.Join(" & ", invitation.Attendees.Select(a => a.FirstName));
                    string rsvpLink = $"{invitationBaseUrl}{invitation.PublicId}";

                    await SendMailJetTemplateEmail(client, invitation.SendTo, guestFirstNames, rsvpLink);
                    invitation.SentTimeStamp = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task SendMailJetTemplateEmail(MailjetClient client, string to, string guestName, string rsvpLink)
        {
            MailjetRequest request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            }
                .Property(Send.Messages, new JArray
                {
                    new JObject
                    {
                        {
                            "From", new JObject
                            {
                                { "Email", "invitation@ciaraandianwedding.ie" },
                                { "Name", "Ciara and Ian" }
                            }
                        },
                        {
                            "To", new JArray
                            {
                                new JObject
                                {
                                    { "Email", to }
                                }
                            }
                        },
                        { "TemplateID", 5814953 },
                        { "TemplateLanguage", true },
                        { "Subject","Invitation - Ciara & Ian's Wedding" },
                        {
                            "Variables", new JObject
                            {
                                { "guest_name", guestName },
                                { "rsvp_link", rsvpLink }
                            }
                        }
                    }
                });
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.GetData());
            }
            else
            {
                string error =
                    $"Error sending email to {to}. {response.StatusCode} - {response.GetErrorInfo()} - {response.GetData()} - {response.GetErrorMessage()}";
                throw new Exception(error);
            }
        }


        [HttpGet]
        public async Task<IActionResult> SendReminders()
        {
            var invitationsWithoutResponse = await _context.Invitations
                .Include(a => a.Attendees)
                .Include(a => a.Rsvp)
                .Where(a => a.Rsvp == null)
                .Take(100)
                .ToListAsync();

            if (invitationsWithoutResponse.Count > 0)
            {
                var invitationBaseUrl = _configuration.GetValue<string>("InvitationBaseUrl");
                MailjetClient client = new MailjetClient(
                    _configuration.GetValue<string>("MailJetApiKey"),
                    _configuration.GetValue<string>("MailJetApiSecret"));

                foreach (var invitation in invitationsWithoutResponse)
                {
                    string guestFirstNames = string.Join(" & ", invitation.Attendees.Select(a => a.FirstName));
                    string rsvpLink = $"{invitationBaseUrl}{invitation.PublicId}";

                    await SendMailJetReminderEmail(client, invitation.SendTo, guestFirstNames, rsvpLink);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task SendMailJetReminderEmail(MailjetClient client, string to, string guestName, string rsvpLink)
        {
            MailjetRequest request = new MailjetRequest
                {
                    Resource = SendV31.Resource,
                }
                .Property(Send.Messages, new JArray
                {
                    new JObject
                    {
                        {
                            "From", new JObject
                            {
                                { "Email", "invitation@ciaraandianwedding.ie" },
                                { "Name", "Ciara and Ian" }
                            }
                        },
                        {
                            "To", new JArray
                            {
                                new JObject
                                {
                                    { "Email", to }
                                }
                            }
                        },
                        { "TemplateID", 6002754 },
                        { "TemplateLanguage", true },
                        { "Subject","RSVP Reminder - Ciara & Ian's Wedding" },
                        {
                            "Variables", new JObject
                            {
                                { "guest_name", guestName },
                                { "rsvp_link", rsvpLink }
                            }
                        }
                    }
                });
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.GetData());
            }
            else
            {
                string error =
                    $"Error sending email to {to}. {response.StatusCode} - {response.GetErrorInfo()} - {response.GetData()} - {response.GetErrorMessage()}";
                throw new Exception(error);
            }
        }

    }
}
