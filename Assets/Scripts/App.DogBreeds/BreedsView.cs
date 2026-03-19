using System.Threading;
using Core;
using UnityEngine;
using Zenject;

namespace App
{
	public class BreedsView : DefaultWindow<BreedsView>
	{
		[SerializeField]
		PooledList breedsPool;

		[SerializeField]
		ObjectSwitcher loaderSwitcher;

		[SerializeField]
		ObjectSwitcher loadedSwitcher;

		[SerializeField]
		ObjectSwitcher failSwitcher;

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

		public void Retry()
		{
			_token.Cancel();
			Clear();
			_ = RequestWeather();
		}

		void Clear()
		{
			loaderSwitcher?.Switch(true);
			loadedSwitcher?.Switch(false);
			failSwitcher?.Switch(false);

			breedsPool?.Flush();
		}

		async Awaitable RequestWeather()
		{
			_token = new();
			var breed = await _controller.Request(_token);
			if (_token.IsCancellationRequested)
			{
				Clear();
				return;
			}

			if (!breed.success)
			{
				failSwitcher.Switch(true);
				loaderSwitcher?.Switch(false);
				loadedSwitcher?.Switch(false);
				return;
			}

			failSwitcher?.Switch(false);
			loaderSwitcher?.Switch(false);
			loadedSwitcher?.Switch(true);

			for (int i = 0; i < 10; i++)
			{
				if (breed.result.data.Length - 1 < i)
					break;

				BreedButtonView view = await breedsPool.GetAsComponent<BreedButtonView>();
				view.Init(breed.result.data[i]);
			}
		}
	}
}