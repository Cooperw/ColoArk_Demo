using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ColoArk.DAL;
using ColoArk.Filters;
using ColoArk.Models;
using ColoArk.Services;
using ColoArk.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColoArk.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserService _userService;
        private readonly IRepository<GiveawayReceipt> _giveawayReceiptRepository;
        private readonly IRepository<Mailbox> _mailboxRepository;
        private readonly IRepository<GiveawayDrop> _giveawayDropRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IEmailSender _emailSender;
        IHostingEnvironment _env;

        public AdminController(IRepository<User> userRepository, IRepository<GiveawayReceipt> giveawayReceiptRepository,
            IRepository<Mailbox> mailboxRepository, IRepository<GiveawayDrop> giveawayDropRepository,
            IRepository<Post> postRepository, IEmailSender emailSender, IHostingEnvironment env)
        {
            this._userRepository = userRepository;
            this._userService = new UserService(this._userRepository);
            this._giveawayReceiptRepository = giveawayReceiptRepository;
            this._mailboxRepository = mailboxRepository;
            this._giveawayDropRepository = giveawayDropRepository;
            this._postRepository = postRepository;
            _emailSender = emailSender;
            _env = env;
        }

        public IActionResult Index(string changeAction)
        {
            ViewBag.changeAction = changeAction;
            return View(_userRepository.AllQueryable().Where(o => o.AuthLevel == Models.User.AuthType.Admin).ToList());
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult ManagePosts(string changeAction)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                ViewBag.changeAction = changeAction;
                return View(_postRepository.AllQueryable().OrderByDescending(o => o.Date).ToList());
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult CreatePost()
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                CreateEditPostViewModel viewModel = new CreateEditPostViewModel();
                IEnumerable<string> pics = System.IO.Directory.EnumerateFiles(_env.WebRootPath + "\\images\\", "*", System.IO.SearchOption.AllDirectories);
                List<string> modifiedPics = new List<string>();
                foreach (string item in pics)
                {
                    modifiedPics.Add(item.Substring(item.LastIndexOf('\\')+1));
                }
                ViewBag.Pictures = modifiedPics;
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(CreateEditPostViewModel viewModel)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                if (ModelState.IsValid)
                {
                    Post post = new Post();
                    post.Date = DateTime.Now;
                    post.Description = viewModel.Description;
                    post.Picture = viewModel.Picture;
                    post.Subtitle = viewModel.Subtitle;
                    post.Title = viewModel.Title;
                    post.Type = viewModel.Type;

                    this._postRepository.Add(post);

                    return RedirectToAction("ManagePosts", new { changeAction = "created" });
                }
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult EditPost(int id)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                Post drop = this._postRepository.Get(id);
                CreateEditPostViewModel viewModel = new CreateEditPostViewModel(drop);
                IEnumerable<string> pics = System.IO.Directory.EnumerateFiles(_env.WebRootPath + "\\images\\", "*", System.IO.SearchOption.AllDirectories);
                List<string> modifiedPics = new List<string>();
                foreach (string item in pics)
                {
                    modifiedPics.Add(item.Substring(item.LastIndexOf('\\') + 1));
                }
                ViewBag.Pictures = modifiedPics;
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(CreateEditPostViewModel viewModel)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                if (ModelState.IsValid)
                {
                    Post post = _postRepository.Get(viewModel.ID);
                    post.Date = DateTime.Now;
                    post.Description = viewModel.Description;
                    post.Picture = viewModel.Picture;
                    post.Subtitle = viewModel.Subtitle;
                    post.Title = viewModel.Title;
                    post.Type = viewModel.Type;

                    this._postRepository.Update(post);

                    return RedirectToAction("ManagePosts", new { changeAction = "updated" });
                }
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                Post post = this._postRepository.Get(id);

                this._postRepository.Delete(post);

                return RedirectToAction("ManagePosts", "Admin", new { changeAction = "deleted" });
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        public IActionResult UploadImage()
        {
            return View();
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(UploadImageViewModel viewmodel)
        {
            if (viewmodel.File == null || viewmodel.File.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot", "images",
                        viewmodel.Filename);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await viewmodel.File.CopyToAsync(stream);
            }

            return RedirectToAction("ManagePosts", "Admin", new { changeAction = "uploaded" });
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult ManageUsers(string changeAction)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                ViewBag.changeAction = changeAction;
                return View(_userRepository.AllQueryable().OrderBy(o => o.AuthLevel).ToList());
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult ImplantTable()
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                return View(_userRepository.AllQueryable().OrderBy(o => o.AuthLevel).ToList());
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult EditUser(int id)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                IEnumerable<string> pics = System.IO.Directory.EnumerateFiles(_env.WebRootPath + "\\images\\", "*", System.IO.SearchOption.AllDirectories);
                List<string> modifiedPics = new List<string>();
                foreach (string item in pics)
                {
                    modifiedPics.Add(item.Substring(item.LastIndexOf('\\') + 1));
                }
                ViewBag.Pictures = modifiedPics;
                return View(new AdminEditUserViewModel(_userRepository.Get(id)));
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(AdminEditUserViewModel viewModel)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                if (ModelState.IsValid)
                {
                    User user = _userRepository.Get(viewModel.ID);
                    user.ImplantID = viewModel.ImplantID;
                    user.Pin = viewModel.Pin;
                    user.PSN = viewModel.PSN;
                    user.AuthLevel = viewModel.AuthLevel;
                    user.Email = viewModel.Email;
                    user.IsDonator = viewModel.IsDonator;
                    if (user.AuthLevel == Models.User.AuthType.Admin)
                    {
                        user.Bio = viewModel.Bio;
                        user.ProfilePic = viewModel.ProfilePic;
                    }

                    _userRepository.Update(user);

                    return RedirectToAction("ManageUsers", "Admin", new { changeAction = "updated" });
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult ManageGiveaway(string changeAction, string type)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                ViewBag.changeAction = changeAction;
                ViewBag.type = type;
                if (!string.IsNullOrEmpty(type) && type.Equals("all"))
                {
                    return View(_giveawayReceiptRepository.AllQueryable().Include("User").Include("Mailbox").OrderByDescending(o => o.EntryDate));
                }
                else
                {
                    return View(_giveawayReceiptRepository.AllQueryable().Include("User").Include("Mailbox").Where(o => !o.IsInMailbox && !o.IsPickedUp).OrderBy(o => o.EntryDate));
                }
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveawayReadyForPickup(int id)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                GiveawayReceipt receipt = _giveawayReceiptRepository.Get(id);
                receipt.ReadyForPickUpDate = DateTime.Now;
                receipt.IsInMailbox = true;
                receipt.IsPickedUp = false;
                _giveawayReceiptRepository.Update(receipt);

                Mailbox mailbox = _mailboxRepository.Get(receipt.MailboxID);
                mailbox.ArrivalDate = receipt.ReadyForPickUpDate;
                _mailboxRepository.Update(mailbox);

                User user = _userRepository.Get(receipt.UserID);

                //Send Email
                string body = "<!DOCTYPE html> <html> <head> <meta charset=\"UTF-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> <link rel=\"stylesheet\" href=\"https://www.w3schools.com/w3css/4/w3.css\"> <link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Oswald\"> <link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Open Sans\"> <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\"> <link rel=\"stylesheet\" href=\"~/lib/bootstrap/dist/css/bootstrap.min.css\" /> <link rel=\"icon\" href=\"~/images/ark_icon.jpg\"> </head> <body> <div class=\"w3-margin-left\"> <style> h1, h2, h3, h4, h5, h6 { font-family: \"Oswald\" } body { font-family: \"Open Sans\" } </style> <h1 style=\"color: black; \">Hello " + user.PSN + ",</h1> <h3 style=\"color: black; \">Thanks for playing on the ColoArk Server.</h3> <h3 style=\"color: black; \">Your " + receipt.Description + " is ready for pickup at mailbox " + mailbox.VaultNumber + " and will be availble until " + receipt.ReadyForPickUpDate.AddDays(7) + ".</h3> <p style=\"color: black; \">Don't forget to process your pickup on the giveaway page. Have a great day, see you in game.</p> <p style=\"color: black; \">-CWSharkbones</p> </div> </body> </html>[" + DateTime.Now.ToString("H:mm:ss mm/dd/yyyy") + "] End of message.";
                await _emailSender.SendEmailAsync(user.Email, "ColoArk Prize - Ready For Pickup", body);

                return RedirectToAction("ManageGiveaway", "Admin", new { changeAction = "delivered" });
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult ManageGiveawayDrops(string changeAction)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                ViewBag.changeAction = changeAction;
                return View(_giveawayDropRepository.All().OrderBy(o => o.Type));
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult CreateGiveawayDrop()
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                CreateEditGiveawayDropViewModel viewModel = new CreateEditGiveawayDropViewModel();
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateGiveawayDrop(CreateEditGiveawayDropViewModel viewModel)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                if (ModelState.IsValid)
                {
                    GiveawayDrop drop = new GiveawayDrop();
                    drop.Name = viewModel.Name;
                    drop.LowerBound = viewModel.LowerBound;
                    drop.HigherBound = viewModel.HigherBound;
                    drop.Type = viewModel.Type;

                    this._giveawayDropRepository.Add(drop);

                    return RedirectToAction("ManageGiveawayDrops", new { changeAction = "created" });
                }
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        public IActionResult EditGiveawayDrop(int id)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                GiveawayDrop drop = this._giveawayDropRepository.Get(id);
                CreateEditGiveawayDropViewModel viewModel = new CreateEditGiveawayDropViewModel(drop);
                return View(viewModel);

            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGiveawayDrop(CreateEditGiveawayDropViewModel viewModel)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                if (ModelState.IsValid)
                {
                    GiveawayDrop drop = new GiveawayDrop();
                    drop.Name = viewModel.Name;
                    drop.LowerBound = viewModel.LowerBound;
                    drop.HigherBound = viewModel.HigherBound;
                    drop.Type = viewModel.Type;

                    this._giveawayDropRepository.Add(drop);

                    return RedirectToAction("ManageGiveawayDrops", new { changeAction = "updated" });
                }
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }

        [Authorize]
        [ServiceFilter(typeof(GlobalFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteGiveawayDrop(int id)
        {
            if (_userService.GetOrCreateUser(User.Identity.Name).AuthLevel == Models.User.AuthType.Admin)
            {
                GiveawayDrop drop = this._giveawayDropRepository.Get(id);

                this._giveawayDropRepository.Delete(drop);

                return RedirectToAction("ManageGiveawayDrops", "Admin", new { changeAction = "deleted" });
            }
            else
            {
                return RedirectToAction("Index", "Admin", new { changeAction = "unauth" });
            }
        }
    }
}