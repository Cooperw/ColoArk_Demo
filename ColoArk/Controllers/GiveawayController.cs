using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColoArk.DAL;
using ColoArk.Filters;
using ColoArk.Models;
using ColoArk.Services;
using ColoArk.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColoArk.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(GlobalFilter))]
    public class GiveawayController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserService _userService;
        private readonly IRepository<GiveawayReceipt> _giveawayReceiptRepository;
        private readonly IRepository<Mailbox> _mailboxRepository;
        private readonly IRepository<GiveawayDrop> _giveawayDropRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IEmailSender _emailSender;

        public GiveawayController(IRepository<User> userRepository, IRepository<GiveawayReceipt> giveawayReceiptRepository, 
            IRepository<Mailbox> mailboxRepository, IRepository<GiveawayDrop> giveawayDropRepository, IRepository<Post> postRepository, IEmailSender emailSender)
        {
            this._userRepository = userRepository;
            this._userService = new UserService(this._userRepository);
            this._giveawayReceiptRepository = giveawayReceiptRepository;
            this._mailboxRepository = mailboxRepository;
            this._giveawayDropRepository = giveawayDropRepository;
            this._postRepository = postRepository;
            _emailSender = emailSender;
        }

        public IActionResult Index(string changeAction)
        {
            ViewBag.changeAction = changeAction;
            List<Post> myPost = _postRepository.AllQueryable().Where(o => o.Type == Post.PostType.Giveaway).ToList();
            if (!myPost.Any())
            {
                return RedirectToAction("Index", "Home", new { changeAction = "underconstruction" });
            }
            ViewBag.GiveawayPost = myPost;
            return View(_giveawayReceiptRepository.AllQueryable().Include("Mailbox").Where(o => o.UserID == _userService.GetOrCreateUser(User.Identity.Name).ID).OrderByDescending(o => o.EntryDate).ToList());
        }

        public IActionResult Enter()
        {
            User TargetUser = _userService.GetOrCreateUser(User.Identity.Name);
            GiveawayReceipt LastPrize = _giveawayReceiptRepository.AllQueryable().Include("Mailbox").Where(o => o.UserID == TargetUser.ID).OrderByDescending(o => o.EntryDate).FirstOrDefault();
            if (LastPrize == null || DateTime.Now.Subtract(LastPrize.EntryDate).TotalDays >= 7 || TargetUser.AuthLevel == Models.User.AuthType.Admin)
            {
                return View();
            }
            return RedirectToAction("Index", "Giveaway", new { changeAction = "early" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enter(EnterGiveawayViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                List<GiveawayDrop> myPossibleDrops = _giveawayDropRepository.AllQueryable()
                    .Where(o => o.Type == viewModel.Type).OrderBy(o => o.Name).ToList();
                Random random = new Random();
                int randomDropNumber = random.Next(0, myPossibleDrops.Count);
                int randomBoundNumber = random.Next(myPossibleDrops[randomDropNumber].LowerBound, myPossibleDrops[randomDropNumber].HigherBound+1);
                string quality = "";
                switch (viewModel.Type)
                {
                    case GiveawayDrop.DropType.Armor:
                    case GiveawayDrop.DropType.Weapon:
                    case GiveawayDrop.DropType.Blueprint:
                        if (randomBoundNumber > 70)
                        {
                            quality = "Ascendant"; //30%
                        }
                        else if (randomBoundNumber > 40)
                        {
                            quality = "Mastercraft"; //30%
                        }
                        else if (randomBoundNumber > 20)
                        {
                            quality = "Journeyman"; //20%
                        }
                        else
                        {
                            quality = "Apprentice"; //20%
                        }
                        break;
                    case GiveawayDrop.DropType.Creature:
                        quality = "Lv" + randomBoundNumber;
                        break;
                    case GiveawayDrop.DropType.Resource:
                    case GiveawayDrop.DropType.Structure:
                        quality = "x" + randomBoundNumber;
                        break;
                }

                //Build Prize Receipt
                GiveawayReceipt prize = new GiveawayReceipt();
                prize.Description = quality + " " + myPossibleDrops[randomDropNumber].Name;
                prize.EntryDate = DateTime.Now;
                prize.IsInMailbox = false;
                prize.IsPickedUp = false;
                prize.ReadyForPickUpDate = prize.EntryDate.AddDays(4);

                //Set User & Mailbox
                User TargetUser = _userService.GetOrCreateUser(User.Identity.Name);
                Mailbox availbleBox = _mailboxRepository.AllQueryable().FirstOrDefault(o => !o.IsActive || DateTime.Now.Subtract(o.ArrivalDate).TotalDays >= 7);
                if (availbleBox == null)
                {
                    availbleBox = new Mailbox();
                    Mailbox highestBox = _mailboxRepository.AllQueryable().OrderByDescending(o => o.VaultNumber).FirstOrDefault();
                    if (highestBox == null)
                    {
                        availbleBox.VaultNumber = 1;
                    }
                    else
                    {
                        availbleBox.VaultNumber = highestBox.VaultNumber + 1;
                    }
                    _mailboxRepository.Add(availbleBox);
                }
                availbleBox.IsActive = true;
                availbleBox.ArrivalDate = prize.ReadyForPickUpDate;
                availbleBox.Mail_Description = prize.Description;
                availbleBox.UserID = TargetUser.ID;
                _mailboxRepository.Update(availbleBox);

                //Set Last Part of Prize
                prize.MailboxID = availbleBox.ID;
                prize.UserID = TargetUser.ID;

                //Add Prize
                _giveawayReceiptRepository.Add(prize);

                //Send Email
                string body = "<!DOCTYPE html> <html> <head> <meta charset=\"UTF-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> <link rel=\"stylesheet\" href=\"https://www.w3schools.com/w3css/4/w3.css\"> <link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Oswald\"> <link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Open Sans\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\"> <link rel=\"stylesheet\" href=\"~/lib/bootstrap/dist/css/bootstrap.min.css\" /> <link rel=\"icon\" href=\"~/images/ark_icon.jpg\"> </head> <body> <div class=\"w3-margin-left\"> <style> h1, h2, h3, h4, h5, h6 { font-family: \"Oswald\" } body { font-family: \"Open Sans\" } </style> <h1 style=\"color: black; \">Hello " + TargetUser.PSN + ",</h1> <h3 style=\"color: black; \">Thanks for playing on the ColoArk Server.</h3> <h3 style=\"color: black; \">Your gift has been added to our delivery list.</h3> <ul class=\"w3-ul w3-hoverable w3-white\" style=\"list-style-type: none;\"> <li class=\"w3-padding-16\"> <table class=\"\"> <tr> <th> Prize </th> <td> "+ prize.Description + " </td> </tr><tr> <th> Status </th> <td> <label class=\"label label-warning\">On Its Way</label> </td> </tr> <tr> <th> Mailbox </th> <td> " + prize.Mailbox.VaultNumber + " </td> </tr> <tr> <th> Expected By </th> <td>" + prize.ReadyForPickUpDate.ToString("MMM dd, yyyy") + "</td> </tr> </table> </li> </ul> <p style=\"color: black; \">Have a great day, see you in game.</p> <p style=\"color: black; \">-CWSharkbones</p> </div> </body> </html>[" + DateTime.Now.ToString("H:mm:ss mm/dd/yyyy")+"] End of message.";
                await _emailSender.SendEmailAsync(TargetUser.Email, "ColoArk Prize - Confirmation", body);

                return RedirectToAction("Index", "Giveaway", new { changeAction = "entered" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPickup(int id)
        {
            GiveawayReceipt receipt = _giveawayReceiptRepository.Get(id);
            receipt.IsInMailbox = false;
            receipt.IsPickedUp = true;
            _giveawayReceiptRepository.Update(receipt);

            Mailbox mailbox = _mailboxRepository.Get(receipt.MailboxID);
            mailbox.IsActive = false;
            _mailboxRepository.Update(mailbox);
            
            return RedirectToAction("Index", "Giveaway", new { changeAction = "pickedup" });
        }

        public IActionResult Drops()
        {
            return View(_giveawayDropRepository.All().OrderBy(o => o.Type));
        }
    }
}