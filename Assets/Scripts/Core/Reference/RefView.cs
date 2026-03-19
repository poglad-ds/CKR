using UnityEngine;

namespace Core
{
	/// <summary>
	/// Interface to pass unity objects to dedicated to them view impl on scene
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRefView<T> where T : UnityEngine.Object
	{
		/// <summary>
		/// Pass a Ref<T> to implementation to show. 
		/// 
		/// Somewhat like bindable, but not - does not observe change in state
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Awaitable Pass(Ref<T> value);

		/// <summary>
		/// Pass already loaded asset
		/// </summary>
		public void Pass(T value);
	}
}