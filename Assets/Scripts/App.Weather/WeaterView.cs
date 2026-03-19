using System.Threading;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App
{
	public class WeatherView : DefaultWindow<WeatherView>, IWindow
	{
		[SerializeReference]
		RefImageView weatherView;

		[SerializeField]
		TMP_Text temperatureText;

		[SerializeField]
		ObjectSwitcher loadingSwitch;
		[SerializeField]
		ObjectSwitcher loadedSwitch;
		[SerializeField]
		ObjectSwitcher failSwitcher;

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

			Clear();
			_ = RequestWeather();
		}

		public void Retry()
		{
			_token.Cancel();
			Clear();
			_ = RequestWeather();
		}

		void Clear()
		{
			loadedSwitch?.Switch(true);
			loadingSwitch?.Switch(false);
			failSwitcher?.Switch(false);

			weatherView?.Dispose();

			if (temperatureText)
				temperatureText.text = string.Empty;
		}

		async Awaitable RequestWeather()
		{
			Clear();

			_token = new();
			var weather = await _controller.Request(_token);

			if (_token.IsCancellationRequested)
			{
				Clear();
				return;
			}

			if (!weather.success)
			{
				loadedSwitch?.Switch(true);
				failSwitcher?.Switch(false);
				loadingSwitch?.Switch(false);
				return;
			}

			loadedSwitch?.Switch(false);
			failSwitcher?.Switch(false);
			loadingSwitch?.Switch(true);

			temperatureText.text = $"{weather.result.temperature}F";
			weatherView?.Pass(await _controller.RequestImage(weather.result.icon, _token));
		}
	}
}