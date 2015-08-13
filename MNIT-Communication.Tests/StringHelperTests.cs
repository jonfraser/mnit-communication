using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MNIT_Communication.Helpers;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MNIT_Communication.Tests
{

	public class StringHelperTests
	{
		[Fact]
		public void StringWithGuidCorrectlyReturnsGuid()
		{
			var guid = Guid.NewGuid();
			var url = "hiweufheiwwhfeui/" + guid.ToString();

			Assert.AreEqual(guid.ToString(), StringHelpers.PullGuidOffEndOfUrl(url));
		}

		[Fact]
		public void StringWithoutGuidCorrectlyReturnsNothing()
		{
			var guid = Guid.NewGuid();
			var url = "hiweufheiwwhfeui/qdwd23udgh2b28d673bdb8237y2d231d2";

			Assert.AreNotEqual(guid.ToString(), StringHelpers.PullGuidOffEndOfUrl(url));
		}
	}
}
