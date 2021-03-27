//Message decoding adapted from: https://github.com/ttu/ruuvitag-sensor
using System;
using System.Linq;
using XamarinRuuviTags.Models;

namespace XamarinRuuviTags.Helpers
{
    public class RuuviDf5Decoder
	{
		public static RuuviTagData DecodeMessage(byte[] message)
		{
			var header = message[0];
			var format = message[2];
			var payload = message.Skip(3).ToArray();
			if (format != 5)
			{
				throw new ArgumentException($"Wrong message format, expected 5 but was {format}");
			}
			//TODO: Implement all fields
			return new RuuviTagData
			{
				Temperature = GetTemp(payload[0], payload[1]),
				Humidity = GetHumidity(payload[2], payload[3]),
				MacAddress = GetMac(payload),
				Battery = GetBattery(payload[12], payload[13]),
				TimeStamp = DateTimeOffset.UtcNow
			};
		}

		private static string GetMac(byte[] payload) =>
			BitConverter.ToString(payload.Skip(17).ToArray()).Replace("-", ":");

		private static double GetTemp(int p1, int p2)
		{
			var value = TwosComplement((p1 << 8) + p2) / 200;
			return Math.Round(value, 2);
		}

		private static double GetHumidity(int p1, int p2)
		{
			if (p1 == 255 && p2 == 255)
			{
				return 0;
			}
			var value = ((double)((p1 & 255) << 8 | p2 & 255)) / 400;
			return Math.Round(value, 2);
		}

		private static double TwosComplement(int value)
		{
			if ((value & (1 << (16 - 1))) != 0)
			{
				value = value - (1 << 16);
			}
			return (double)value;
		}

		private static double GetBattery(int p1, int p2)
		{
			var power = (p1 & 255) << 8 | (p2 & 255);
			var voltage = ((power % 4294967296) >> 5) + 1600;
			if (((power % 4294967296) >> 5) == 194686858891537)
			{
				return 0;
			}
			return Math.Round(((double)voltage) / 1000, 3);
		}
	}
}
