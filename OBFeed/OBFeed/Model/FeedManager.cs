using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Akavache;

namespace OBFeed {
	public class FeedManager {

		#region Singleton

		static FeedManager _Instance;
		public static FeedManager Instance {
			get { return _Instance ?? (_Instance = new FeedManager()); }
		}

		#endregion

		#region Events

		public EventHandler<Feed> OnFeedUpdated;

		#endregion

		#region Properties

		static HttpClient _HttpClient;
		public static HttpClient HttpClient {
			get { return _HttpClient ?? (_HttpClient = new HttpClient()); }
		}

		ObservableCollection<Feed> _MasterFeeds;
		public ObservableCollection<Feed> MasterFeeds {
			get { return _MasterFeeds; }
			set {  _MasterFeeds = value; }
		}

		#endregion

		#region Config

		FeedManager() {

			// Set up Akavache
			try {
				BlobCache.ApplicationName = Constants.Keys.ApplicationName;
			} catch (Exception e) {
				OBDebug.Log("NUnit hates Akavache", e);
			}

			// Load the saved feeds
			MasterFeeds = new ObservableCollection<Feed>();
			Task.Run(async () => await LoadSavedFeeds());
		}

		#endregion

		#region Interface

		public async Task LoadSavedFeeds() {
			OBDebug.Log("Loading saved feeds (async)");
			ObservableCollection<Feed> feeds = null;
			try {
				// Will throw an exception if the object doesn't exist - I really do hate exceptions
				feeds = await BlobCache.LocalMachine.GetObject<ObservableCollection<Feed>>(Constants.Keys.SavedFeeds);
			} catch (Exception e) {
				OBDebug.Log(e);
			}
			if (feeds != null) {
				// Got our previous list of feeds
				OBDebug.Log("Got our last cache of feeds");
				foreach (var feed in feeds) {
					MasterFeeds.Add(feed);
				}
			} else {
				// No existing cached feed list - make a new one
				OBDebug.Log("Making a new set of feeds");
				MasterFeeds.Add(new Feed { Url = Constants.Urls.SkySportsFeed });
				MasterFeeds.Add(new Feed { Url = Constants.Urls.TechRadarFeed });
			}
		}

		public void UpdateFeedInBackground(Feed feed) {
			OBDebug.Log("Scheduling background feed update", feed.Url);
			Task.Run(async () => await UpdateFeed(feed));
		}

		#endregion

		#region Helper

		public async Task UpdateFeed(Feed feed) {
			OBDebug.Log("Updating feed (async)", feed.Url);
			feed.Items.Clear();

			var xmlDocument = await GetParsedRssXml(feed.Url);
			foreach (var item in ParseFeed(xmlDocument)) {
				feed.Items.Add(item);
			}

			OBDebug.Log("Invoking OnFeedUpdated", feed.Url, feed.Items.Count);
			if (OnFeedUpdated != null) {
				OnFeedUpdated(this, feed);
			}
		}

		public static List<FeedItem> ParseFeed(XDocument feedDoc) {
			var items = new List<FeedItem>();

			// Check for nulls
			if (feedDoc == null) return items;

			var rssItems = feedDoc.Descendants("item");
			foreach (var itemElement in rssItems) {
				var title = itemElement.Element("title")?.Value;
				var pubdate = itemElement.Element("pubDate")?.Value;
				var image = itemElement.Element("enclosure")?.Attribute("url")?.Value;
				items.Add(new FeedItem { Title = title, PubDate = pubdate, Image = image });
			}
			return items;
		}
	
		public static async Task<XDocument> GetParsedRssXml(string url) {
			// If we're within 15 minutes, grab a cached copy of the url response
			string rawXml = string.Empty;
			try {
				rawXml = await BlobCache.LocalMachine.GetOrFetchObject(url, async () => await DownloadStringAtUrl(url), DateTimeOffset.UtcNow.AddMinutes(15));
			} catch (Exception e) {
				OBDebug.Log("Exception while using akavache", "Attempting manual download instead", e);
				rawXml = await DownloadStringAtUrl(url);
			}

			OBDebug.Log("Got raw xml response", url);
			return XDocument.Parse(rawXml);
		}

		public static async Task<string> DownloadStringAtUrl(string url) {
			OBDebug.Log("Downloading string at url", url);
			return await HttpClient.GetStringAsync(url);
		}

		#endregion
	}
}