using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core
{
	[CreateAssetMenu(menuName = "App/Controllers/Windows", fileName = "WindowController")]
	public class WindowController : ScriptableObjectInstaller
	{
		HashSet<IWindow> registered = new(8);
		HashSet<IWindow> opened = new(8);

		public bool Open(IWindow window)
		{
			registered.TryGetValue(window, out var actualWindow);

			if (actualWindow is null)
				return false;

			CloseAll();
			actualWindow.OnOpen();
			opened.Add(actualWindow);
			return true;
		}

		public bool Close(IWindow window)
		{
			registered.TryGetValue(window, out var actualWindow);

			if (actualWindow is null)
				return false;

			actualWindow.OnClose();
			opened.Remove(actualWindow);
			return true;
		}

		public void CloseAll()
		{
			foreach (var window in opened)
				window.OnClose();

			opened.Clear();
		}

		public void Register(IWindow window)
		{
			registered.Add(window);
		}

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}
	}
}