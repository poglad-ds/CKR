using System.Threading;
using Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace App
{
	public class WeatherView : DefaultWindow, IWindow
	{
		[SerializeReference]
		RefImageView weatherView;

		[SerializeField]
		TMP_Text temperatureText;

		[SerializeField]
		ObjectSwitcher loadingSwitch;
		[SerializeField]
		ObjectSwitcher loadedSwitch;

		WeatherControllerSettings _controller;

		CancellationTokenSource _token;

		[Inject]
		public void Inject(WeatherControllerSettings controller)
		{
			_controller = controller;
		}

		public override void OnClose()
		{
			base.OnClose();

			_token?.Cancel();
			Clear();
		}

		public override void OnOpen()
		{
			base.OnOpen();

			_token = new();
			Clear();
			_ = RequestWeather();
		}

		void Clear()
		{
			loadedSwitch?.Switch(true);
			loadingSwitch?.Switch(false);

			weatherView?.Dispose();

			if (temperatureText)
				temperatureText.text = string.Empty;
		}

		async Awaitable RequestWeather()
		{
			_token = new();
			var weather = await _controller.Request(_token.Token);

			if (!weather.IsValid)
				return;

			loadedSwitch?.Switch(false);
			loadingSwitch?.Switch(true);

			weatherView?.Pass(await _controller.RequestImage(weather.icon, _token.Token));
			temperatureText.text = $"{weather.temperature}F";
		}
	}
}