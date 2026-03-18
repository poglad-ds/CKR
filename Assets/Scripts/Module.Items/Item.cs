using System;
using UnityEngine;

namespace Module.Items
{
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

	[Serializable]
	public struct SerializableItem
	{
		public ItemData Data;

		public long Count;

		public Item AsItem()
		{
			return new() { Data = Data, Count = Count };
		}
	}
}