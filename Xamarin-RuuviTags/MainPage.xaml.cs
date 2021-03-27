using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinRuuviTags.ViewModels;

namespace Xamarin_RuuviTags
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
        protected override async void OnAppearing()
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            base.OnAppearing();
        }
    }
}
