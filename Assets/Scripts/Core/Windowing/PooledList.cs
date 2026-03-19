using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;


namespace Core
{
	/// <summary>
	/// Classic object pool - load from referenced asset, instantiate expected amount in its own object pool
	/// </summary>
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
		DiContainer _container;

		public void Start()
		{
			_instantiated.Capacity = expected;

			_ = Initialize();
		}

		/// <summary>
		/// Request new GameObject from pool
		/// </summary>
		public virtual async Awaitable<GameObject> Get()
		{
			if (!_initialization.Task.IsCompleted)
				await _initialization.Task;

			if (_unused.Any())
			{
				var obj = _unused.Pop();
				_used.Add(obj);

				obj.SetActive(true);
				return obj;
			}

			return await Instantiate(true);
		}

		/// <summary>
		/// Request new GO as casted to some component. Dangerous!
		/// </summary>
		public virtual async Awaitable<T> GetAsComponent<T>()
		{
			var obj = await Get();
			return obj.GetComponent<T>();
		}

		public virtual bool Put(GameObject go)
		{
			if (!_used.TryGetValue(go, out var actual))
				return false;

			actual.SetActive(false);
			_used.Remove(actual);
			_unused.Push(actual);
			return true;
		}

		public virtual void Flush()
		{
			foreach (var go in _used)
			{
				go.SetActive(false);
				_unused.Push(go);
			}

			_used.Clear();
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

			try
			{
				_container.InjectGameObject(instantiated);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				throw;
			}

			_instantiated.Add(instantiated);

			if (alreadyUsing)
			{
				_used.Add(instantiated);
				instantiated.SetActive(true);
			}
			else
			{
				_unused.Push(instantiated);
				instantiated.SetActive(false);
			}


			return instantiated;
		}

		[Inject]
		void Inject(DiContainer container)
		{
			_container = container;
		}
	}
}