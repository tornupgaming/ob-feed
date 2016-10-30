using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using OBFeed;

namespace OBUnitTests {
	
	[TestFixture]
	public class FeedManagerTests {
		
		[Test]
		public void ParseFeedNullDocumentNoException() 
		{
			FeedManager.ParseFeed(null);
		}

		[Test]
		public void ParseFeedEmptyDocumentNoException() 
		{
			var doc = XDocument.Parse("<xml></xml>");
			FeedManager.ParseFeed(doc);
		}

		[Test]
		public void ParseFeedItemParsesCorrectly() 
		{
			var doc = XDocument.Parse("<xml><item><title>title</title><pubDate>pub</pubDate><enclosure url=\"image\"></enclosure></item></xml>");
			var items = FeedManager.ParseFeed(doc);
			Assert.That(items.Single(i => i.Title == "title" && i.PubDate=="pub" && i.Image=="image") != null);
		}

		[Test]
		public void ParseFeedEmptyImageParsesCorrectly() 
		{
			var doc = XDocument.Parse("<xml><item><title>title</title><pubDate>pub</pubDate></item></xml>");
			var items = FeedManager.ParseFeed(doc);
			Assert.That(items.Single(i => i.Title == "title" && i.PubDate == "pub" && string.IsNullOrEmpty(i.Image)) != null);
		}

		[Test]
		public async Task DownloadStringWorksCorrectly() 
		{
			var result = await FeedManager.DownloadStringAtUrl("http://www.google.com");
			Assert.That(!string.IsNullOrWhiteSpace(result));
		}

		[Test]
		public async Task UpdateFeedFiresUpdateEvent() 
		{
			bool fired = false;
			var feed = new Feed { Url = Constants.Urls.SkySportsFeed };
			FeedManager.Instance.OnFeedUpdated += (sender, e) => fired = e == feed;
			await FeedManager.Instance.UpdateFeed(feed);
			Assert.That(fired == true);
		}
	}
}
