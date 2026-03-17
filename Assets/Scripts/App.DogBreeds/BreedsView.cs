using UnityEngine;
using Zenject;

namespace App
{
	public class BreedsView : MonoBehaviour
	{
		BreedsController _controller;

		[Inject]
		public void Inject(BreedsController controller)
		{
			_controller = controller;
			_ = RequestWeather();
		}

		async Awaitable RequestWeather()
		{
			var breed = await _controller.RequestData();

			Debug.Log(breed.attributes.name);
		}
	}
}