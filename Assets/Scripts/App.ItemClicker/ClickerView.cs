using Module.Items;
using UnityEngine;
using Zenject;
using System;
using UnityEngine.UI;
using Core;

namespace App
{
	public class ClickerView : DefaultWindow
	{
		[SerializeField]
		Button manualClicker;

		ClickerController _controller;

		public void Start()
		{
			manualClicker?.onClick.AddListener(Click);
		}

		[Inject]
		public void Initialize(ClickerController settings)
		{
			_controller = settings;

			if (_controller)
				return;

			_controller.Clicked += OnClicked;
		}

		void Click()
		{
			_controller?.TryClick();
		}

		void OnClicked()
		{
			_controller.TryClick();
		}

		void OnDestroy()
		{
			if (_controller)
				_controller.Clicked -= OnClicked;
		}
	}
}