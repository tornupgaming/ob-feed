using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Akavache;
using Xamarin.Forms;

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

		HttpClient _HttpClient;
		public HttpClient HttpClient {
			get { return _HttpClient ?? (_HttpClient = new HttpClient()); }
		}

		ObservableCollection<Feed> _MasterFeeds;
		public ObservableCollection<Feed> MasterFeeds {
			get { return _MasterFeeds; }
			set { if (value != _MasterFeeds) { _MasterFeeds = value; } }
		}

		#endregion

		#region Config

		FeedManager() {

			// Set up Akavache
			BlobCache.ApplicationName = Constants.Keys.ApplicationName;

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

		#endregion

		#region Helper

		public void UpdateFeedInBackground(Feed feed) {
			OBDebug.Log("Scheduling background feed update", feed.Url);
			Task.Run(async () => await UpdateFeed(feed));
		}

		async Task UpdateFeed(Feed feed) {
			OBDebug.Log("Updating feed (async)", feed.Url);
			feed.Items.Clear();

			var xmlDocument = await GetParsedRssXml(feed.Url);
			var rssItems = xmlDocument.Descendants("item");
			foreach (var itemElement in rssItems) {
				var title = itemElement.Element("title")?.Value;
				var pubdate = itemElement.Element("pubDate")?.Value;
				var image = itemElement.Element("enclosure")?.Attribute("url")?.Value; feed.Items.Add(new FeedItem { Title = title, PubDate = pubdate, Image = image });
			}

			Device.BeginInvokeOnMainThread(() => {
				OBDebug.Log("Invoking OnFeedUpdated", feed.Url, feed.Items.Count);
				if (OnFeedUpdated != null) {
					OnFeedUpdated(this, feed);
				}
			});
		}
	
		async Task<XDocument> GetParsedRssXml(string url) {
			// If we're within 15 minutes, grab a cached copy of the url response
			var rawXml = await BlobCache.LocalMachine.GetOrFetchObject(url, async () => await DownloadStringAtUrl(url), DateTimeOffset.UtcNow.AddMinutes(15));
			OBDebug.Log("Got raw xml response", url);
			return XDocument.Parse(rawXml);
		}

		async Task<string> DownloadStringAtUrl(string url) {
			OBDebug.Log("Downloading string at url", url);
			return await HttpClient.GetStringAsync(url);
		}

		#endregion
	}
}