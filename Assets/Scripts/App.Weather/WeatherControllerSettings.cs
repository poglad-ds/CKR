using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core;
using Core.Web;
using UnityEngine;
using Zenject;

namespace App
{
	[CreateAssetMenu(menuName = "App/Controllers/Weather", fileName = "WeatherSettings")]
	public class WeatherControllerSettings : ScriptableObjectInstaller, IRequestSender<WeatherData>
	{

		[SerializeField]
		string uri;

		[SerializeField]
		Ref<Sprite> fallbackSprite;

		public Ref<Sprite> FallbackSprite => fallbackSprite;

		WeatherResponseData _cachedLatestResponce = new();

		public async Awaitable<(bool success, WeatherData result)> Request(CancellationTokenSource cancellationToken)
		{
			using var request = await WebRequest.CreateGet(uri).Send(WebRequestSendSettings.Default, cancellationToken);

			if (!request)
				return (false, null);

			var resp = request.ParseAsJson(_cachedLatestResponce);

			return (true, resp.data.properties.periods.FirstOrDefault());
		}

		public async Awaitable<Sprite> RequestImage(string uri, CancellationTokenSource cancellationToken)
		{
			using var request = await WebRequest.CreateGet(uri).Send(WebRequestSendSettings.Default, cancellationToken);

			if (!request)
				return null;

			return request.ParseAsSprite().data;
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