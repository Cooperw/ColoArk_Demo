using ColoArk.DAL;
using ColoArk.Models;
using ColoArk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ColoArk.Filters
{
    public class GlobalFilter : IResultFilter
    {
        private readonly UserService _userService;

        public GlobalFilter(IRepository<User> userRepository)
        {
            this._userService = new UserService(userRepository);
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var controller = context.Controller as Controller;
            string test = controller.User.Identity.Name;
            controller.ViewBag.user = this._userService.GetOrCreateUser(controller.User.Identity.Name);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // pass
        }
    }
}
