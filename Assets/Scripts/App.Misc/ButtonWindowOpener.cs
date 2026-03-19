using Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App
{
	[RequireComponent(typeof(Button))]
	public class ButtonWindowSwitcher : MonoBehaviour
	{
		[SerializeField]
		DefaultWindow target;

		Button _button;
		WindowController _controller;

		void Start()
		{
			_button = GetComponent<Button>();

			_button.onClick.AddListener(Action);
		}

		void Action()
		{
			if (!target)
			{
				_controller.CloseAll();
				return;
			}

			if (!target.IsOpen)
				_controller.Open(target);
			else
				_controller.Close(target);
		}

		[Inject]
		void Inject(WindowController controller)
		{
			_controller = controller;
		}
	}
}