using UnityEngine;
using Zenject;

namespace Core
{
	[RequireComponent(typeof(Canvas))]
	public abstract class DefaultWindow : MonoBehaviour, IWindow
	{
		[SerializeField]
		bool openByDefault = false;

		public Canvas Canvas => _canvas;

		public bool IsOpen => _canvas.enabled;

		protected Canvas _canvas;
		protected WindowController _windowController;

		public virtual void Awake()
		{
			_canvas = GetComponent<Canvas>();

			_windowController.Register(this);
			if (openByDefault)
				_windowController.Open(this);
			else
				_windowController.Close(this);
		}

		public virtual void OnOpen()
		{
			_canvas.enabled = true;
		}

		public virtual void OnClose()
		{
			_canvas.enabled = false;
		}

		[Inject]
		protected void InjectBase(WindowController controller)
		{
			_windowController = controller;
		}
	}
}