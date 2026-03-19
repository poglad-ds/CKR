using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App
{
	public class BreedView : MonoInstaller, IWindow
	{
		[SerializeField]
		bool openByDefault = false;

		public Canvas Canvas => _canvas;

		public bool IsOpen => _canvas.enabled;

		protected Canvas _canvas;
		protected WindowController _windowController;


		[SerializeField]
		TMP_Text breedNameText;

		[SerializeField]
		TMP_Text breedDescriptonText;

		public virtual void Awake()
		{
			_canvas = GetComponent<Canvas>();

			_windowController.Register(this);
		}

		void OnEnable()
		{
			if (breedNameText)
				breedNameText.text = string.Empty;

			if (breedDescriptonText)
				breedDescriptonText.text = string.Empty;
		}

		public void Init(BreedData data)
		{
			_windowController.Open(this);

			breedNameText.text = data.attributes.name;
			breedDescriptonText.text = data.attributes.description;
		}

		public void OnOpen()
		{
			_canvas.enabled = true;
		}

		public void OnClose()
		{
			_canvas.enabled = false;
		}

		[Inject]
		public void Inject(WindowController controller)
		{
			_windowController = controller;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}
	}
}