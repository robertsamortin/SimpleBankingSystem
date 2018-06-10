﻿using BankingSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BankingSystem
{
    public class UserManager : IUserManager
    {
        UsersDataAccess users = new UsersDataAccess();

        public UserManager()
        {
        }

        public Users GetCurrentUser(HttpContext httpContext)
        {
            string currentUserId = this.GetCurrentUserId(httpContext).ToString();

            if (currentUserId == null)
                return null;

            return users.GetUserByAccountNumber(currentUserId);
        }

        public string GetCurrentUserId(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return null;

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
                return null;

            string currentUserId = claim.Value;
            return currentUserId;
        }

        public async void SignIn(HttpContext httpContext, Users user, bool isPersistent = false)
        {
            ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = isPersistent }
            );
        }

        public async void SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public Users Validate(string LoginName, string Password)
        {
            var user = users.UserLogin(LoginName, Password);

            return user;
        }

        
        private IEnumerable<Claim> GetUserClaims(Users user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.AccountNumber.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.LoginName));
            return claims;
        }
    }
}
