using Module.Items;
using UnityEngine;
using Zenject;
using System;
using UnityEngine.UI;
using Core;

namespace App
{
	public class ClickerView : DefaultWindow<ClickerView>
	{
		[SerializeField]
		Button manualClicker;

		[SerializeField]
		PooledList pool;

		ClickerController _controller;

		public override void Start()
		{
			base.Start();

			manualClicker?.onClick.AddListener(Click);
		}

		[Inject]
		public void Initialize(ClickerController settings)
		{
			_controller = settings;

			if (_controller)
				return;

		}

		void Click()
		{
			_controller?.TryClick();
			_ = PlayAttraction();
		}

		async Awaitable PlayAttraction()
		{
			var view = await pool.GetAsComponent<ClickItemView>();
			await view.Run();
			pool.Put(view.gameObject);
		}

		void OnDestroy()
		{
		}
	}
}