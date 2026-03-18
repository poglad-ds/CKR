using System;
using System.Linq;
using Core.Web;
using UnityEngine;
using Zenject;

namespace App
{
	[CreateAssetMenu(menuName = "App/Controllers/Weather", fileName = "Weather")]
	public class WeatherControllerSettings : ScriptableObjectInstaller
	{
		[SerializeField]
		string uri;

		WeatherResponseData _cachedLatestResponce = new();

		public async Awaitable<WeatherData> RequestData()
		{
			using var request = await WebRequest.CreateGet(uri).Send(WebRequestSendSettings.Default);

			if (!request)
				return null;

			var resp = request.Parse(_cachedLatestResponce);

			return resp.data.properties.periods.FirstOrDefault();
		}

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}
	}

	//
	// Было 2 выбора - решил по итогу просто повторить структуру классом вместо затягивания Newtonsoft.Json
	//

	[Serializable]
	public class WeatherResponseData : IJsonObject
	{
		public WeatherPropertiesData properties;
	}

	[Serializable]
	public class WeatherPropertiesData
	{
		public WeatherData[] periods;
	}

	[Serializable]
	public class WeatherData
	{
		public int number = -1;
		public string name;

		public string startTime;
		public string endTime;

		public bool isDaytime;
		public int temperature;
		public string temperatureUnit;

		public string temperatureTrend;

		public string windSpeed;
		public string windDirection;
		public string icon;
		public string shortForecast;
		public string detailedForecast;

		public bool IsValid => number > 0;
	}
}