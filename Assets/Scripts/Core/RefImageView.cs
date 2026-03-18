using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	[RequireComponent(typeof(Image))]
	public class RefImageView : MonoBehaviour, IRefView<Sprite>
	{
		Image _component;

		Sprite _fallback;
		Ref<Sprite> _current;

		public void Awake()
		{
			if (!_component)
				Cache();
		}

		public async Awaitable Pass(Ref<Sprite> value)
		{
			if (!_component && !Cache())
				return;

			if (_current is not null)
			{
				_current.Disposed -= OnRefDisposed;
			}

			_current = value;
			_current.Disposed += OnRefDisposed;
			var asset = await value.LoadAsset();
			if (!asset)
				return;

			_component.sprite = asset;
		}

		void OnRefDisposed()
		{
			_current.Disposed -= OnRefDisposed;
			_component.sprite = _fallback;
		}

		bool Cache()
		{
			_component = GetComponent<Image>();
			if (!_component)
				return false;

			_fallback = _component.sprite;
			return true;
		}
	}
}