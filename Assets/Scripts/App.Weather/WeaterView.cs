using UnityEngine;
using Zenject;

namespace App
{
	public class WeatherView : MonoBehaviour, IWeatherControllerInjecter
	{
		WeatherController _controller;

		[Inject]
		public void Inject(WeatherController controller)
		{
			_controller = controller;
			_ = RequestWeather();
		}

		async Awaitable RequestWeather()
		{
			var weather = await _controller.RequestWeatherData();

			if (!weather.IsValid)
				return;

			Debug.Log(weather.temperature);
		}
	}
}