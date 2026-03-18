using Module.Items;
using UnityEngine;
using Zenject;
using System;
using UnityEngine.UI;

namespace App
{
	public class ClickerControllerView : MonoBehaviour
	{
		[SerializeField]
		Button manualClicker;

		ClickerController _settings;

		void Start()
		{
			manualClicker.onClick.AddListener(Click);
		}

		[Inject]
		public void Initialize(ClickerController settings)
		{
			_settings = settings;

			if (_settings)
				return;

			_settings.Clicked += OnClicked;
		}

		void Click()
		{
			_settings?.TryClick();
		}

		void OnClicked()
		{

		}

		void OnDestroy()
		{
			if (_settings)
				_settings.Clicked -= OnClicked;
		}
	}
}