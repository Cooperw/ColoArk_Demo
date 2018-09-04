using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColoArk.DAL;
using ColoArk.Filters;
using ColoArk.Models;
using ColoArk.Services;
using ColoArk.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ColoArk.Controllers
{
    [ServiceFilter(typeof(GlobalFilter))]
    public class UserController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserService _userService;
        IHostingEnvironment _env;

        public UserController(IRepository<User> userRepository, IHostingEnvironment env)
        {
            this._userRepository = userRepository;
            this._userService = new UserService(this._userRepository);
            _env = env;
        }

        public IActionResult Index(string changeAction)
        {
            ViewBag.changeAction = changeAction;
            return View(_userService.GetOrCreateUser(User.Identity.Name));
        }

        public IActionResult Edit(int id)
        {
            IEnumerable<string> pics = System.IO.Directory.EnumerateFiles(_env.WebRootPath + "\\images\\", "*", System.IO.SearchOption.AllDirectories);
            List<string> modifiedPics = new List<string>();
            foreach (string item in pics)
            {
                modifiedPics.Add(item.Substring(item.LastIndexOf('\\') + 1));
            }
            ViewBag.Pictures = modifiedPics;
            return View(new EditUserViewModel(_userRepository.Get(id)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User user = _userRepository.Get(viewModel.ID);
                user.ImplantID = viewModel.ImplantID;
                user.Pin = viewModel.Pin;
                user.PSN = viewModel.PSN;
                if (user.AuthLevel == Models.User.AuthType.Admin)
                {
                    user.Bio = viewModel.Bio;
                    user.ProfilePic = viewModel.ProfilePic;
                }

                _userRepository.Update(user);

                return RedirectToAction("Index", "User", new { changeAction = "updated" });
            }
            return View();
        }
    }
}