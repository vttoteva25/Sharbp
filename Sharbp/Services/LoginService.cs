using Models;
using Sharbp.HelpingTools;
using System;
using System.Linq;
namespace Sharbp.Services
{
	public class LoginService
    {
        private readonly JsonFileUserService _userService;

        public LoginService(JsonFileUserService userService)
        {
            this._userService = userService;
        }
        
        public User Login(string username, string password)
        {
            if(_userService.Users.Any(x=>x.Username==username))
            {
                if(_userService.Users.Find(x => x.Username == username).Password==Hasher.Hash(password))
                {
                    return _userService.Users.Find(x => x.Username == username);
                }
                else
                {
                    throw new ArgumentException("Incorrect username or password");
                }
            }
            else
            {
                throw new ArgumentException("Incorrect username or password");
            }
        }
    }
}
