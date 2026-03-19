namespace Core
{
	public interface IWindow
	{
		public bool IsOpen { get; }

		public void OnOpen();
		public void OnClose();
	}
}