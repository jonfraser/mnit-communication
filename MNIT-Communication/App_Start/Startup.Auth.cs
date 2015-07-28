﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using MNIT_Communication.Models;
using Microsoft.WindowsAzure;
using MNIT_Communication.Controllers;
using MNIT_Communication.Helpers.CustomSignIn;

namespace MNIT_Communication
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
				
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
					//OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
					//	validateInterval: TimeSpan.FromMinutes(30),
					//	regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });  
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);          
#if DEBUG
            app.UseDevelopmentAuthentication(new DevelopmentAuthenticationOptions
            {
                UserId = "1",
                UserName = "perskest",
                Email = "sjperske@gmail.com",
                Phone = "0400099743"
            });
#else
            app.UseMicrosoftAccountAuthentication(
				clientId: CloudConfigurationManager.GetSetting("MicrosoftAuthClientID"),
				clientSecret: CloudConfigurationManager.GetSetting("MicrosoftAuthClientSecret"));
			app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
			{
				ClientId = CloudConfigurationManager.GetSetting("GoogleAuthClientID"),
				ClientSecret = CloudConfigurationManager.GetSetting("GoogleAuthClientSecret")
			});

#endif



            app.MapSignalR();
        }
    }
}