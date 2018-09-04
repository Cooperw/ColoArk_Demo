using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ColoArk.Models;
using ColoArk.DAL;
using ColoArk.Services;

namespace ColoArk.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserService _userService;
        private readonly IRepository<Post> _postRepository;

        public HomeController(IRepository<User> userRepository, IRepository<Post> postRepository)
        {
            this._userRepository = userRepository;
            this._userService = new UserService(this._userRepository);
            this._postRepository = postRepository;
        }

        public IActionResult Index(string changeAction, Post.PostType? type)
        {
            ViewBag.changeAction = changeAction;
            List<Post> model;
            switch (type)
            {
                case Post.PostType.AdminWar:
                    model = _postRepository.All().OrderByDescending(o => o.Date)
                        .Where(o => o.Type == Post.PostType.AdminWar).ToList();
                    break;
                case Post.PostType.Rules:
                    model = _postRepository.All().OrderByDescending(o => o.Date)
                        .Where(o => o.Type == Post.PostType.Rules).ToList();
                    break;
                case Post.PostType.Giveaway:
                    model = _postRepository.All().OrderByDescending(o => o.Date)
                        .Where(o => o.Type == Post.PostType.Giveaway).ToList();
                    break;
                case Post.PostType.Labyrinth:
                    model = _postRepository.All().OrderByDescending(o => o.Date)
                        .Where(o => o.Type == Post.PostType.Labyrinth).ToList();
                    break;
                case Post.PostType.ScavengerHunt:
                    model = _postRepository.All().OrderByDescending(o => o.Date)
                        .Where(o => o.Type == Post.PostType.ScavengerHunt).ToList();
                    break;
                case Post.PostType.General:
                default:
                    model = _postRepository.All().OrderByDescending(o => o.Date)
                        .Where(o => o.Type == Post.PostType.General).ToList();
                    break;
            }
            if (!model.Any() && (string.IsNullOrEmpty(changeAction) || !changeAction.Equals("underconstruction")))
            {
                return RedirectToAction("Index", "Home", new { changeAction = "underconstruction" });
            }
            return View(model);
        }
    }
}
