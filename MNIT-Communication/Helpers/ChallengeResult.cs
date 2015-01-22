﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace MNIT_Communication.Helpers
{
	public class ChallengeResult : HttpUnauthorizedResult
	{
		private const string XsrfKey = "XsrfId";

		public ChallengeResult(string provider, string redirectUri)
			: this(provider, redirectUri, null)
		{
		}

		public ChallengeResult(string provider, string redirectUri, string userId)
		{
			LoginProvider = provider;
			RedirectUri = redirectUri;
			UserId = userId;
		}

		public string LoginProvider { get; set; }
		public string RedirectUri { get; set; }
		public string UserId { get; set; }

		public override void ExecuteResult(ControllerContext context)
		{
			context.RequestContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

			var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
			if (UserId != null)
			{
				properties.Dictionary[XsrfKey] = UserId;
			}
			context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
		}
	}
}