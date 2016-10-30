using Xamarin.Forms;

namespace OBFeed {
	public partial class FeedPage : ContentPage {
		public FeedPage() {
			InitializeComponent();
			BindingContext = new FeedPageViewModel { Feeds = FeedManager.Instance.MasterFeeds };
		}
	}
}
