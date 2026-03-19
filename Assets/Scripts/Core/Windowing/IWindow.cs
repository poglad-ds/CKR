namespace Core
{
	/// <summary>
	/// Application scope Window
	/// 
	/// Usually derive from DefaultWindow<T>, this is worth only for edge-case
	/// </summary>
	public interface IWindow
	{
		public bool IsOpen { get; }

		public void OnOpen();
		public void OnClose();
	}
}