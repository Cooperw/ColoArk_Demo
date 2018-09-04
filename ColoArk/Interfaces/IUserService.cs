using ColoArk.Models;

namespace ColoArk.Interfaces
{
    interface IUserService
    {
        User GetOrCreateUser(string email);
    }
}
