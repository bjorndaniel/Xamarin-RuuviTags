using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinRuuviTags.Messages;
using XamarinRuuviTags.Models;
using XamarinRuuviTags.Services;

namespace XamarinRuuviTags.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private RuuviService _service;
        public MainPageViewModel()
        {
            Title = "About";
            _service = DependencyService.Get<RuuviService>();
            StartScanCommand = new Command(async () =>
            {
                IsBusy = true;
                await _service.TryGetReading();
            });
            MessagingCenter.Subscribe<ScanMessage>(this, "", (m) =>
            {
                try
                {
                    var tagString = Preferences.Get("Tags", "");
                    var tags = JsonSerializer.Deserialize<List<RuuviTagData>>(tagString);
                    if (tags?.Any() ?? false)
                    {
                        Tags.Clear();
                        Tags.AddRange(tags);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        public ObservableRangeCollection<RuuviTagData> Tags { get; set; } = new ObservableRangeCollection<RuuviTagData>();

        public ICommand StartScanCommand { get; }
    }
}
