using Core;
using PrimeTween;
using UnityEngine;

namespace App
{
	public class ClickItemView : MonoBehaviour
	{
		[SerializeField]
		RefImageView refImageView;

		public async Awaitable Run()
		{
			((RectTransform)transform).anchoredPosition = Vector2.zero;

			//welp tweener break material instance anyway, so do not care much here
			refImageView.Component.color = new(1, 1, 1, 0.5f);

			var rect = refImageView.Component.canvas.pixelRect;
			await Tween.LocalPosition(transform, new(Random.Range(-rect.width * 0.5f * 0.75f, rect.width * 0.5f * 0.75f), Random.Range(rect.height * 0.5f * 0.45f, rect.height * 0.5f * 0.75f), 0), duration: 3f).Group(Tween.Alpha(refImageView.Component, settings: new(1f, new(duration: 0.2f))))
			.Chain(Tween.ShakeLocalPosition(transform, new(5, 3, 0), duration: 1f, frequency: 2f).Group(Tween.Alpha(refImageView.Component, settings: new(0f, new(duration: 0.2f)))));
		}
	}
}