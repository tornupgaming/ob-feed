using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace OBFeed {
	public partial class FeedPage : ContentPage {

		ObservableCollection<FeedItem> _ObservableItemCollection;

		const string SkySportsFeedUrl = "http://www1.skysports.com/feeds/11095/news.xml";
		const string TechRadarFeedUrl = "http://www.techradar.com/rss";

		string[] _AvailableFeeds = {
			TechRadarFeedUrl,
			SkySportsFeedUrl
		};

		HttpClient _HttpClient;
		public HttpClient HttpClient {
			get { return _HttpClient ?? (_HttpClient = new HttpClient()); }
		}

		public FeedPage() {
			InitializeComponent();
			lview_feed_items.ItemsSource = _ObservableItemCollection = new ObservableCollection<FeedItem>();
			Task.Run(async () => await UpdateFeed());
		}

		async Task UpdateFeed() {
			_ObservableItemCollection.Clear();

			foreach (var feedUrl in _AvailableFeeds) {
				var xmlDocument = await GetParsedRssXml(feedUrl);
				var rssItems = xmlDocument.Descendants("item");
				foreach (var itemElement in rssItems) {
					var title = itemElement.Element("title")?.Value;
					var pubdate = itemElement.Element("pubDate")?.Value;
					var image = itemElement.Element("enclosure")?.Attribute("url")?.Value;
					_ObservableItemCollection.Add(new FeedItem { Title = title, PubDate = pubdate, Image = image });
			}
			}
		}

		async Task<XDocument> GetParsedRssXml(string url) {
			var rawXml = await this.HttpClient.GetStringAsync(url);
			return XDocument.Parse(rawXml);
		}
	}
}
