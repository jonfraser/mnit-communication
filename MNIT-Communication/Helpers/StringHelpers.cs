using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNIT_Communication.Helpers
{
	public static class StringHelpers
	{
		public static string PullGuidOffEndOfUrl(string url)
		{
			return url.Substring(url.Length - Guid.Empty.ToString().Length);
		}
	}
}