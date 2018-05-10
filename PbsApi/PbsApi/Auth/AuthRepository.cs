using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PbsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PbsApi.Auth
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;
        //private pbsEntities.login_info  _ctx;
	    private UserManager<IdentityUser> _userManager;
        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(login_info loginInfo)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = loginInfo.user_name
            };
            var result = await _userManager.CreateAsync(user, loginInfo.user_pwd);
            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}