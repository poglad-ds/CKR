using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	public class RefViewSprite : MonoBehaviour, IRefView<Sprite>
	{
		[SerializeField]
		Image target;

		public async Awaitable Pass(Ref<Sprite> value)
		{
			if (!target)
				return;

			var asset = await value.LoadAsset();
			target.sprite = asset;
		}
	}
}