using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Core;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using Unity.Collections;

namespace Module.Items
{
	public interface IItemData
	{
		public Ref<Sprite> Icon { get; }

		public string Name { get; }
	}

	/// <summary>
	/// Item representation for transactions
	/// 
	/// Data - SO metadata about item
	/// Count - current amount per defined player stash.
	/// </summary>
	public struct Item
	{
		//8 + 8 = 16
		public IItemData Data;
		public long Count;
	}

	[CreateAssetMenu(menuName = "App/Controllers/Inventory", fileName = "Inventory")]
	public class Inventory : ScriptableObjectInstaller
	{
		/// <summary>
		/// Inventory - source
		/// Item - global count after update for this specific item
		/// </summary>
		public static event Action<Inventory, Item> Updated;

		[SerializeField]
		Item[] initialItems = Array.Empty<Item>();

		public Dictionary<IItemData, Item> items = new();

		void Initialize()
		{
			//Pretty much anything can be here - deserialization, etc... For this demo this is enough

			foreach (var item in initialItems)
				items.Add(item.Data, item);
		}

		/// <summary>
		/// Process an operation on item in inventory with provided delta
		/// </summary>
		/// <returns>Operation status. Failed only in error case</returns>
		public bool TryChange(Item delta)
		{
			if (delta.Data is null)
				return false;

			if (items.TryGetValue(delta.Data, out var item))
			{
				try
				{
					item.Count += checked(delta.Count);
				}
				catch
#if DEBUG
				(OverflowException ex)
#endif
				{
#if DEBUG
					Debug.LogError($"Catch illegal overflow mul for item {delta.Data.Name} with value {delta.Count}, trace {ex.StackTrace}");
#endif
					return false;
				}

				if (item.Count <= 0)
				{
					item.Count = 0;
				}

				items[delta.Data] = item;
				Updated?.Invoke(this, item);

				return true;
			}

			if (delta.Count < 0)
				return false;

			items.Add(delta.Data, delta);
			Updated?.Invoke(this, delta);
			return true;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}
	}
}