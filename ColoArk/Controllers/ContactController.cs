using ColoArk.DAL;
using ColoArk.Filters;
using ColoArk.Models;
using ColoArk.Services;
using ColoArk.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColoArk.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(GlobalFilter))]
    public class ContactController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserService _userService;
        private readonly IEmailSender _emailSender;

        public ContactController(IRepository<User> userRepository, IEmailSender emailSender)
        {
            this._userRepository = userRepository;
            this._userService = new UserService(this._userRepository);
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexContactViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User TargetUser = _userService.GetOrCreateUser(User.Identity.Name);
                string body = $"<!DOCTYPE html> <html> <head> <meta charset=\"UTF-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> <link rel=\"stylesheet\" href=\"https://www.w3schools.com/w3css/4/w3.css\"> <link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Oswald\"> <link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Open Sans\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\"> <link rel=\"stylesheet\" href=\"~/lib/bootstrap/dist/css/bootstrap.min.css\" /> <link rel=\"icon\" href=\"~/images/ark_icon.jpg\"> </head> <body> <div class=\"w3-margin-left\"> <style> h1, h2, h3, h4, h5, h6 {{ font-family: \"Oswald\" }} body {{ font-family: \"Open Sans\" }} </style> <table><tr><td><strong>Email</strong></td><td>{TargetUser.Email}</td></tr> <tr><td><strong>PSN</strong></td><td>{TargetUser.PSN}</td></tr> <tr><td><strong>ImplantID</strong></td><td>{TargetUser.ImplantID}</td></tr> <tr><td><strong>Reason</strong></td><td>{viewModel.Reason}</td></tr> <tr><td><strong>Prefered Contact</strong></td><td>{viewModel.PreferredContact}</td></tr> <tr><td><strong>Subject</strong></td><td>{viewModel.Subject}</td></tr> <tr><td><strong>Description</strong></td><td>{viewModel.Description}</td></tr> </table></div> </body> </html>[" + DateTime.Now.ToString("H:mm:ss mm/dd/yyyy") + "] End of message.";
                await _emailSender.SendEmailAsync("coloarkserver@gmail.com", "Contact - "+viewModel.Reason, body);

                return RedirectToAction("Index", "Home", new { changeAction = "contactsent" });
            }
            return View();
        }

    }
}
