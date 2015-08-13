using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json;
using Owin;
using MNIT_Communication.App_Start;
using AppBuilderExtensions = Owin.AppBuilderExtensions;

namespace MNIT_Communication
{
    public static class DevelopmentAuthenticationExtensions
    {
        public static IAppBuilder UseDevelopmentAuthentication(this IAppBuilder app,
            DevelopmentAuthenticationOptions options)
        {
            app.Use((object) typeof (DevelopmentAuthenticationMiddleware), (object) app, (object) options);
            return app;
        }

        public static IDataProtectionProvider GetDataProtectionProvider(this IAppBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            object obj;
            if (app.Properties.TryGetValue("security.DataProtectionProvider", out obj))
            {
                Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>> create =
                    obj as Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>>;
                if (create != null)
                    return (IDataProtectionProvider) new CallDataProtectionProvider(create);
            }
            return (IDataProtectionProvider) null;
        }

        public static IDataProtector CreateDataProtector(this IAppBuilder app, params string[] purposes)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            else
                return
                    (GetDataProtectionProvider(app) ??
                     FallbackDataProtectionProvider(app)).Create(purposes);
        }

        private static IDataProtectionProvider FallbackDataProtectionProvider(IAppBuilder app)
        {
            return (IDataProtectionProvider) new DpapiDataProtectionProvider(GetAppName(app));
        }

        private static string GetAppName(IAppBuilder app)
        {
            object obj;
            if (app.Properties.TryGetValue("host.AppName", out obj))
            {
                string str = obj as string;
                if (!string.IsNullOrEmpty(str))
                    return str;
            }
            throw new NotSupportedException();
        }
    }

    internal class CallDataProtectionProvider : IDataProtectionProvider
    {
        private readonly Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>> _create;

        public CallDataProtectionProvider(Func<string[], Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>>> create)
        {
            this._create = create;
        }

        public IDataProtector Create(params string[] purposes)
        {
            Tuple<Func<byte[], byte[]>, Func<byte[], byte[]>> tuple = this._create(purposes);
            return (IDataProtector)new CallDataProtection(tuple.Item1, tuple.Item2);
        }

        private class CallDataProtection : IDataProtector
        {
            private readonly Func<byte[], byte[]> _protect;
            private readonly Func<byte[], byte[]> _unprotect;

            public CallDataProtection(Func<byte[], byte[]> protect, Func<byte[], byte[]> unprotect)
            {
                this._protect = protect;
                this._unprotect = unprotect;
            }

            public byte[] Protect(byte[] userData)
            {
                return this._protect(userData);
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                return this._unprotect(protectedData);
            }
        }
    }

    
    public class DevelopmentAuthenticationMiddleware : AuthenticationMiddleware<DevelopmentAuthenticationOptions>
    {
        public DevelopmentAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, DevelopmentAuthenticationOptions options)
            : base(next, options)
        {
            if (options.StateDataFormat == null)
            {
                options.StateDataFormat = (ISecureDataFormat<AuthenticationProperties>)new PropertiesDataFormat(app.CreateDataProtector(typeof(GoogleOAuth2AuthenticationMiddleware).FullName, this.Options.AuthenticationType, "v1"));
            }
        }

        protected override AuthenticationHandler<DevelopmentAuthenticationOptions> CreateHandler()
        {
            return (AuthenticationHandler<DevelopmentAuthenticationOptions>)new DevelopmentAuthenticationHandler();
        }
    }

    public class DevelopmentAuthenticationOptions : AuthenticationOptions
    {
        public DevelopmentAuthenticationOptions()
            : base("Development")
        {
            this.Caption = "Development";
            this.CallbackPath = new PathString("/signin-development");
            this.AuthenticationMode = AuthenticationMode.Passive;
            this.AuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
        }

        public string Caption
        {
            get
            {
                return this.Description.Caption;
            }
            set
            {
                this.Description.Caption = value;
            }
        }

        public PathString CallbackPath { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }
        
        public string Phone { get; set; }

        public string SignInAsAuthenticationType { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }

    public class DevelopmentAuthenticationHandler : AuthenticationHandler<DevelopmentAuthenticationOptions>
    {

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return await Task.Run(() =>
            {
                var identity = new ClaimsIdentity(Options.SignInAsAuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Options.UserId, null, Options.Caption, Options.Caption));
                identity.AddClaim(new Claim(ClaimTypes.Name, Options.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Email, Options.Email));
                identity.AddClaim(new Claim(ClaimTypes.HomePhone, Options.Phone));

                var properties = Options.StateDataFormat.Unprotect(Request.Query["state"]);

                return new AuthenticationTicket(identity, properties);
            });
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

                // Only react to 401 if there is an authentication challenge for the authentication
                // type of this handler.
                if (challenge != null)
                {
                    var state = challenge.Properties;

                    if (string.IsNullOrEmpty(state.RedirectUri))
                    {
                        state.RedirectUri = Request.Uri.ToString();
                    }

                    var stateString = Options.StateDataFormat.Protect(state);

                    Response.Redirect(WebUtilities.AddQueryString(Options.CallbackPath.Value, "state", stateString));
                }
            }

            return Task.FromResult<object>(null);
        }

        public override async Task<bool> InvokeAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                var ticket = await AuthenticateAsync();

                if (ticket != null)
                {
                    Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

                    Response.Redirect(ticket.Properties.RedirectUri);

                    // Prevent further processing by the owin pipeline.
                    return true;
                }
            }
            // Let the rest of the pipeline run.
            return false;
        }


    }
   
}
