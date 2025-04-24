using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SmartEcommerce.Controllers.Helper
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); 
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            SmartEcommerce.Models.Admin.Logins user = new BusinessLogic.Accounts().ValidateCustomer(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
            }
            else if (user.LoginType == 1)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                identity.AddClaim(new Claim("loginid", user.LoginId.ToString()));
                identity.AddClaim(new Claim("username", user.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                context.Validated(identity);
            }
            else if (user.LoginType == 2)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                identity.AddClaim(new Claim("loginid", user.LoginId.ToString()));
                identity.AddClaim(new Claim("username", user.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                context.Validated(identity);
            }
            else if (user.LoginType == 3)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "customer"));
                identity.AddClaim(new Claim("loginid", user.LoginId.ToString()));
                identity.AddClaim(new Claim("username", user.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
            }
        }
        
    }
}