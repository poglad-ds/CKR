using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Core
{
	[Serializable]
	public class Ref<T> : IDisposable where T : UnityEngine.Object
	{
		public event Action Disposed;

		[SerializeField]
		AssetReferenceT<T> Data;

		public RefOperationStatus Status { get; private set; }

		List<GameObject> _instantiated = new();
		T _loaded;

		/// <summary>
		/// Load asset into memory. Should be accessable to usage from this moment
		/// </summary>
		/// <returns>Loaded asset or null if failed</returns>
		public async Awaitable<T> LoadAsset()
		{
			if (_loaded)
				return _loaded;

			Status = RefOperationStatus.InProgress;

			var handle = Addressables.LoadAssetAsync<T>(Data);
			await handle.Task;

			Status = handle.Status switch
			{
				UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed => RefOperationStatus.Failed,
				UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded => RefOperationStatus.Succeeded,
				UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.None => RefOperationStatus.InProgress,
				_ => RefOperationStatus.Failed
			};

#if DEBUG
			if (Status is RefOperationStatus.Failed)
				Debug.LogError(handle.OperationException);
#endif

			_loaded = handle.Result;
			return _loaded;
		}

		public void ManuallyPassLoaded(T asset)
		{
			Dispose();
			_loaded = asset;
		}

		public async Awaitable<T> InstantiateAsComponent(Transform parent)
		{
			var go = await Instantiate(parent);
			T value = go.GetComponent<T>();
			return value;
		}

		/// <summary>
		/// Create new GameObject of this loaded asset
		/// </summary>
		/// <param name="parent">Optional transform parent</param>
		/// <returns>Instantiated object, but in component form</returns>
		public async Awaitable<GameObject> Instantiate(Transform parent)
		{
			await LoadAsset();
			var handler = Addressables.InstantiateAsync(Data, parent);
			await handler.Task;

			var value = handler.Result;
			_instantiated.Add(value);
			return value;
		}


		public void Dispose()
		{
			Disposed?.Invoke();
			_loaded = null;

			foreach (var item in _instantiated)
				UnityEngine.Object.Destroy(item);
			_instantiated.Clear();

			Addressables.Release(Data);
		}
	}

	public enum RefOperationStatus
	{
		None,
		InProgress,
		Failed,
		Succeeded
	}
}