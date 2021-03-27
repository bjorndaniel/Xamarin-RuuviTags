using System;
using System.Text.Json.Serialization;

namespace XamarinRuuviTags.Models
{
    public class RuuviTagData
	{
		public double Temperature { get; set; }
		public double Humidity { get; set; }
		public string MacAddress { get; set; } = "";
		public double Battery { get; set; }
		public DateTimeOffset TimeStamp { get; set; }
		[JsonIgnore]
		public string TemperatureDisplay => $"Temp: {Temperature}°C";
		[JsonIgnore]
		public string HumidityDisplay => $"Humidity: {Humidity}%";
		[JsonIgnore]
		public string BatteryDisplay => $"Battery: {Battery}V";
		[JsonIgnore]
		public string TimeStampDisplay => $"Timestamp: {TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")} UTC";
	}
}
