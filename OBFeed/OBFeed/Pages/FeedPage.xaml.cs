using Xamarin.Forms;

namespace OBFeed {
	public partial class FeedPage : ContentPage {
		public FeedPage() {
			InitializeComponent();
			BindingContext = new FeedPageViewModel { Feeds = FeedManager.Instance.MasterFeeds };

			// Disabling Selection
			lview_feed_items.ItemSelected += (sender, e) => ((ListView)sender).SelectedItem = null;
		}
	}
}
