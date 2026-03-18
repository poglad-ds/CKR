using System;
using UnityEngine;
using Zenject;
using Core;
using System.Collections.Generic;
using System.Linq;

namespace Module.Items
{
	[CreateAssetMenu(menuName = "App/Controllers/Inventory", fileName = "Inventory")]
	public class InventoryController : ScriptableObjectInstaller
	{
		/// <summary>
		/// Item - this inventory item count after update for this specific item
		/// </summary>
		public event Action<Item> Updated;

		[SerializeField]
		SerializableItem[] initialItems = Array.Empty<SerializableItem>();

		public IReadOnlyCollection<SerializableItem> InitialItems => initialItems;

		public IReadOnlyList<Item> Items => items.Select(x => x.Value).ToList();

		Dictionary<IItemData, Item> items = new();

		[Inject]
		public void Initialize()
		{
			//Pretty much anything can be here - deserialization, etc... For this demo this is enough

			foreach (var item in InitialItems)
				items.Add(item.Data, item.AsItem());
		}

		public long Count(in IItemData item)
		{
			if (items.TryGetValue(item, out var value))
				return value.Count;

			return 0;
		}

		/// <summary>
		/// Process an operation on item in inventory with provided delta
		/// </summary>
		/// <returns>Operation status. Failed only in error case</returns>
		public bool TryChange(in Item delta)
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
				Updated?.Invoke(item);

#if DEBUG_ITEMS
				Debug.Log($"Changed {delta.Data.Name} for {delta.Count} amount, in the end remain {item.Count}");
#endif
				return true;
			}

			if (delta.Count < 0)
				return false;

			items.Add(delta.Data, delta);
			Updated?.Invoke(delta);

#if DEBUG_ITEMS
				Debug.Log($"Added {delta.Data.Name} for {delta.Count} amount");
#endif
			return true;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}
	}
}