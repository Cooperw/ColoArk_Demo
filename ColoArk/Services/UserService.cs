using ColoArk.DAL;
using ColoArk.Interfaces;
using ColoArk.Models;
using System.Linq;

namespace ColoArk.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            this._userRepository = userRepository;
        }

        public User GetOrCreateUser(string email)
        {
            User user = this._userRepository.All().FirstOrDefault(u => u.Email.Equals(email));

            if (user == null)
            {
                User newUser = new User();
                if (email.Equals("cooperww32@gmail.com"))
                {
                    newUser.AuthLevel = User.AuthType.Admin;
                }
                else
                {
                    newUser.AuthLevel = User.AuthType.Player;
                }
                newUser.Email = email;
                newUser.ImplantID = 0;
                newUser.IsDonator = false;
                newUser.Pin = 0;
                newUser.PSN = "";

                this._userRepository.Add(newUser);
                return newUser;
            }

            return user;
        }
    }
}
