using UnityEngine;
using Zenject;

namespace Core
{
	public abstract class DefaultWindow : MonoInstaller, IWindow
	{
		[SerializeField]
		protected bool openByDefault = false;

		protected Canvas _canvas;

		public bool IsOpen => _canvas.enabled;

		public virtual void Awake()
		{
			_canvas = GetComponent<Canvas>();
		}

		public virtual void OnOpen()
		{
			_canvas.enabled = true;
		}

		public virtual void OnClose()
		{
			_canvas.enabled = false;
		}
	}

	[RequireComponent(typeof(Canvas))]
	public abstract class DefaultWindow<T> : DefaultWindow where T : DefaultWindow<T>
	{
		public Canvas Canvas => _canvas;

		protected WindowController _windowController;

		public override void Awake()
		{
			base.Awake();

			_windowController.Register(this);

			if (openByDefault)
				_windowController.Open(this);
			else
				_windowController.Close(this);
		}

		[Inject]
		protected void InjectBase(WindowController controller)
		{
			_windowController = controller;
		}

		public override void InstallBindings()
		{
			Container.BindInstance<T>((T)this).AsSingle();
		}
	}
}