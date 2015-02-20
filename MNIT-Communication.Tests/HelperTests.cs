using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MNIT_Communication.Helpers;

namespace MNIT_Communication.Tests
{
	[TestClass]
	public class HelperTests
	{
		[TestMethod]
		public async Task StringWithGuidCorrectlyReturnsGuid()
		{
			var guid = Guid.NewGuid();
			var url = "hiweufheiwwhfeui/" + guid.ToString();

			Assert.AreEqual(guid.ToString(), StringHelpers.PullGuidOffEndOfUrl(url));
		}

		[TestMethod]
		public async Task StringWithoutGuidCorrectlyReturnsNothing()
		{
			var guid = Guid.NewGuid();
			var url = "hiweufheiwwhfeui/qdwd23udgh2b28d673bdb8237y2d231d2";

			Assert.AreNotEqual(guid.ToString(), StringHelpers.PullGuidOffEndOfUrl(url));
		}
	}
}
