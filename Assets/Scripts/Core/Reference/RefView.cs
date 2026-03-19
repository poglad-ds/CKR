using UnityEngine;

namespace Core
{
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

		public void Pass(T value);
	}
}