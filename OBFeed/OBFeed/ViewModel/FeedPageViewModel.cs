using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OBFeed {
	public class FeedPageViewModel : OBNotifyClass {

		#region Services

		// Any injected services would be stored here
		// This project doesn't contain any IoC nor does it currently need to

		#endregion

		#region Properties

		bool _IsRefreshing;
		public bool IsRefreshing {
			get { return _IsRefreshing; }
			set { if (value != _IsRefreshing) { _IsRefreshing = value; Notify(); } }
		}

		string _ListViewPlaceholderText;
		public string ListViewPlaceholderText {
			get { return _ListViewPlaceholderText; }
			set { if (value != _ListViewPlaceholderText) { _ListViewPlaceholderText = value; Notify(); } }
		}

		string _SearchTerm;
		public string SearchTerm {
			get { return _SearchTerm; }
			set { if (value != _SearchTerm) { _SearchTerm = value; Notify(); } }
		}

		ObservableCollection<FeedItem> _ItemsToShow;
		public ObservableCollection<FeedItem> ItemsToShow {
			get { return _ItemsToShow; }
			set { if (value != _ItemsToShow) { _ItemsToShow = value; Notify(); } }
		}

		ObservableCollection<Feed> _Feeds;
		public ObservableCollection<Feed> Feeds {
			get { return _Feeds; }
			set { if (value != _Feeds) {
					if(_Feeds != null) _Feeds.CollectionChanged -= OnFeedsCollectionChanged;
					if (value != null) value.CollectionChanged += OnFeedsCollectionChanged;
					_Feeds = value;
					Notify(); 
				} }
		}

		#endregion

		#region Config

		public FeedPageViewModel() 
		{
			// Init neededs vars
			SearchTerm = string.Empty;
			ListViewPlaceholderText = string.Empty;
			ItemsToShow = new ObservableCollection<FeedItem>();

			FeedManager.Instance.OnFeedUpdated += OnFeedUpdated;

			// Setup any listeners we might need
			Bind<string>(nameof(SearchTerm), OnSearchTermPropertyChanged);
			Bind<ObservableCollection<Feed>>(nameof(Feeds), OnFeedsPropertyChanged);
		}

		~FeedPageViewModel() {
			FeedManager.Instance.OnFeedUpdated -= OnFeedUpdated;
			Feeds = null;
		}

		#endregion

		#region Property Changed Handlers

		void OnFeedsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) 
		{
			if (e.Action == NotifyCollectionChangedAction.Add) {
				foreach (var feed in e.NewItems) {
					FeedManager.Instance.UpdateFeedInBackground(feed as Feed);
				}
			}
		}

		void OnSearchTermPropertyChanged(string term) 
		{
			UpdateShownItems ();
		}

		void OnFeedsPropertyChanged(ObservableCollection<Feed> feeds) 
		{
			UpdateShownItems ();
		}

		void OnFeedUpdated(object sender, Feed feed) 
		{
			UpdateShownItems();
		}

		#endregion

		#region Func.

		public Command OnShouldRefresh { get { return new Command(async () => await _OnShouldRefresh()); } }

		async Task _OnShouldRefresh() 
		{
			foreach (var feed in Feeds) {
				await FeedManager.Instance.UpdateFeed(feed);
			}
			IsRefreshing = false;
		}

		void UpdateShownItems() 
		{
			ItemsToShow.Clear();

			if (Feeds == null || Feeds.Count <= 0) { 
				ListViewPlaceholderText = "No Feeds Available";
				return;
			}

			foreach (var item in Feeds.SelectMany(f => f.GetItemsBySearchTerm(SearchTerm))) {
				ItemsToShow.Add(item);
			}

			if (ItemsToShow.Count <= 0) {
				ListViewPlaceholderText = "No Feeds to Show";
			} else {
				ListViewPlaceholderText = string.Empty;
			}
		}

		#endregion
	}
}
