using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Core
{
	public class PooledList : MonoBehaviour
	{
		[SerializeField]
		Ref<GameObject> target;

		[SerializeField]
		int expected = 8;

		List<GameObject> _instantiated = new();
		Stack<GameObject> _unused = new();
		HashSet<GameObject> _used = new();

		TaskCompletionSource<bool> _initialization;

		public void Awake()
		{
			_instantiated.Capacity = expected;

			_ = Initialize();
		}

		public async Awaitable<GameObject> Get()
		{
			if (!_initialization.Task.IsCompleted)
				await _initialization.Task;

			if (_unused.Any())
			{
				var obj = _unused.Pop();
				_used.Add(obj);

				return obj;
			}

			return await Instantiate(true);
		}

		public async Awaitable<T> GetAsComponent<T>() where T : Component
		{
			var obj = await Get();
			return obj.GetComponent<T>();
		}

		async Awaitable Initialize()
		{
			_initialization = new();
			await target.LoadAsset();

			for (int i = 0; i < expected; i++)
			{
				await Instantiate(false);
			}

			_initialization.SetResult(true);
		}

		async Awaitable<GameObject> Instantiate(bool alreadyUsing)
		{
			var instantiated = await target.Instantiate(transform);

			_instantiated.Add(instantiated);

			if (alreadyUsing)
				_used.Add(instantiated);
			else
				_unused.Push(instantiated);

			instantiated.SetActive(false);
			return instantiated;
		}
	}
}