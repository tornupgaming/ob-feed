using System.Threading.Tasks;
using Xamarin.Forms;

namespace OBFeed {
	public partial class SplashPage : ContentPage {
		public SplashPage() {
			InitializeComponent();
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			Task.Run(async () => await PerformInitAsync());
		}

		async Task PerformInitAsync() {
			await Task.Delay(3000);
			Device.BeginInvokeOnMainThread(() => {
				Application.Current.MainPage = new FeedPage();
			});
		}
	}
}
