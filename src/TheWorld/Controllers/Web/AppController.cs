using Microsoft.AspNet.Mvc;
using System;
using System.Linq;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private WorldContext _context;

        public AppController(IMailService mailService, WorldContext context)
        {
            _mailService = mailService;
            _context = context;
        }

        public IActionResult Index()
        {
            var trips = _context.Trips
                                .OrderBy(t => t.Name)
                                .ToList();
            return View(trips);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)  //Server side validation of the model
            {
                var fromEmail = Startup.Configuration["AppSettings:SiteEmailAddress"];

                if (string.IsNullOrWhiteSpace(fromEmail))
                {
                    ModelState.AddModelError("", "Could not send email, configuration problem");
                }

                if (_mailService.SendMail(
                    fromEmail,
                    fromEmail,
                    $"Contact Page from {model.Name} ({model.Email})",
                    model.Message))
                {
                    ModelState.Clear();

                    ViewBag.Message = "Mail Sent. Thanks !";
                }
            }
            return View();
        }
    }
}