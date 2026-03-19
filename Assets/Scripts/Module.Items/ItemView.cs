using System;
using Core;
using Module.Items;
using TMPro;
using UnityEngine;
using Zenject;

namespace App
{
	public class ItemView : MonoBehaviour
	{
		//Well, better remake as itemView... 
		[SerializeField]
		ItemData itemData;

		[SerializeField]
		RefImageView itemSprite;

		[SerializeField]
		TMP_Text itemCount;


		InventoryController _inventory;

		public void OnEnable()
		{
			_inventory.Updated += Refresh;
			Refresh(_inventory.Get(itemData));
		}

		public void OnDisable()
		{
			_inventory.Updated -= Refresh;
		}

		private void Refresh(Item item)
		{
			if (!item.Data.Equals(itemData))
				return;

			_ = itemSprite.Pass(item.Data.Icon);
			itemCount.text = item.Count.ToString();
		}

		[Inject]
		public void Inject(InventoryController inventory)
		{
			_inventory = inventory;
		}
	}
}