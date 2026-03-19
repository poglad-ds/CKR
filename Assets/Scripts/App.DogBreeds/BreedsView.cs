using System.Threading;
using Core;
using UnityEngine;
using Zenject;

namespace App
{
	public class BreedsView : DefaultWindow
	{
		[SerializeField]
		BreedPool breedsPool;

		[SerializeField]
		ObjectSwitcher loaderSwitcher;

		[SerializeField]
		ObjectSwitcher loadedSwitcher;

		BreedsControllerSettings _controller;
		CancellationTokenSource _token;

		[Inject]
		public void Inject(BreedsControllerSettings controller)
		{
			_controller = controller;
		}

		public override void OnOpen()
		{
			base.OnOpen();
			_ = RequestWeather();
		}

		public override void OnClose()
		{
			base.OnClose();

			_token?.Cancel();
			Clear();
		}

		void Clear()
		{
			loaderSwitcher?.Switch(true);
			loadedSwitcher?.Switch(false);

			breedsPool?.Flush();
		}

		async Awaitable RequestWeather()
		{
			_token = new();
			var breed = await _controller.Request(_token.Token);

			if (breed is null)
				return;

			loaderSwitcher?.Switch(false);
			loadedSwitcher?.Switch(true);

			for (int i = 0; i < 10; i++)
			{
				if (breed.data.Length - 1 < i)
					break;

				BreedButtonView view = await breedsPool.GetAsComponent();
				view.Init(breed.data[i]);
			}
		}
	}
}