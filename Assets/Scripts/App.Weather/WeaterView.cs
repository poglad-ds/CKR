using UnityEngine;
using Zenject;

namespace App
{
	public class WeatherView : MonoBehaviour
	{
		WeatherControllerSettings _controller;

		[Inject]
		public void Inject(WeatherControllerSettings controller)
		{
			_controller = controller;
			_ = RequestWeather();
		}

		async Awaitable RequestWeather()
		{
			var weather = await _controller.RequestData();

			if (!weather.IsValid)
				return;

			Debug.Log(weather.temperature);
		}
	}
}