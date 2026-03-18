using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Module.Items
{
	public class InventoryController : MonoInstaller
	{
		/// <summary>
		/// Inventory - source
		/// Item - global count after update for this specific item
		/// </summary>
		public static event Action<InventoryController, Item> Updated;

		public IReadOnlyList<Item> Items => items.Select(x => x.Value).ToList();

		Dictionary<IItemData, Item> items = new();

		[Inject]
		public void Initialize(InventoryControllerSettings settings)
		{
			//Pretty much anything can be here - deserialization, etc... For this demo this is enough

			foreach (var item in settings.InitialItems)
				items.Add(item.Data, item.AsItem());
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