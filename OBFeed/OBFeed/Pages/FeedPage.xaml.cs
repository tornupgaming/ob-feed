using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;

namespace OBFeed {
	public partial class FeedPage : ContentPage {

		FeedPageViewModel _ViewModel;

		public FeedPage() {
			InitializeComponent();
			BindingContext = _ViewModel = new FeedPageViewModel();
			_ViewModel.Feeds = FeedManager.Instance.MasterFeeds;
		}
	}
}
