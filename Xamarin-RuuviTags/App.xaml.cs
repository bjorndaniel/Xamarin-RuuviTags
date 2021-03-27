using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinRuuviTags.Services;

namespace Xamarin_RuuviTags
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DependencyService.Register<RuuviService>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
