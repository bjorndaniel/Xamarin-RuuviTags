using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinRuuviTags.Helpers;
using XamarinRuuviTags.Messages;
using XamarinRuuviTags.Models;

namespace XamarinRuuviTags.Services
{
    public class RuuviService
    {
        private IAdapter _btAdapter;
        private List<RuuviTagData> _tags = new List<RuuviTagData>();
        public RuuviService()
        {
            _btAdapter = CrossBluetoothLE.Current.Adapter;
            _btAdapter.DeviceAdvertised += OnDeviceAdvertised;
            _btAdapter.ScanTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
            _btAdapter.ScanTimeoutElapsed += OnScanTimeout;
        }

        private void OnScanTimeout(object sender, EventArgs e)
        {
            Preferences.Set("Tags", JsonSerializer.Serialize(_tags));
            MessagingCenter.Send(new ScanMessage(), string.Empty);
        }

        private void OnDeviceAdvertised(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            var message = e.Device.AdvertisementRecords.FirstOrDefault(_ => _.Data.Length == 26);
            if (message != null)
            {
                var tm = BitConverter.ToString(message.Data.Take(2).ToArray());
                if (tm != "99-04")//Ruuvi manufacturer id (seems reversed but is working for now)
                {
                    return;
                }
                var tagMessage = RuuviDf5Decoder.DecodeMessage(message.Data);
                Debug.WriteLine(tagMessage.MacAddress);
                Debug.WriteLine(tagMessage.Temperature);
                Debug.WriteLine(tagMessage.Humidity);
                Debug.WriteLine(tagMessage.Battery);
                var exist = _tags.FirstOrDefault(_ => _.MacAddress == tagMessage.MacAddress);
                if (exist != null) //Just save 1 dataset per scan
                {
                    _tags.Remove(exist);
                }
                _tags.Add(tagMessage);
            }
        }

        public async Task TryGetReading()
        {
            try
            {
                _tags = new List<RuuviTagData>();
                if (_btAdapter.IsScanning)
                {
                    await _btAdapter.StopScanningForDevicesAsync();
                }
                await _btAdapter.StartScanningForDevicesAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
