using System;
using Core;
using Module.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

		[SerializeField]
		PooledList sounds;

		InventoryController _inventory;

		void Start()
		{
			if (!_inventory)
				return;

			Refresh(_inventory.Get(itemData));
		}

		public void OnEnable()
		{
			if (!_inventory)
				return;

			_inventory.Updated += Refresh;
			Refresh(_inventory.Get(itemData));
		}

		public void OnDisable()
		{
			if (!_inventory)
				return;

			_inventory.Updated -= Refresh;
		}

		private void Refresh(Item item)
		{
			if (!item.Data.Equals(itemData))
				return;

			_ = itemSprite?.Pass(item.Data.Icon);

			if (itemCount)
				itemCount.text = item.Count.ToString();

			if (sounds)
				_ = PlaySound();
		}

		async Awaitable PlaySound()
		{
			var soundPlayer = await sounds.GetAsComponent<AudioSource>();

			var length = soundPlayer.clip.length;
			soundPlayer.Play();
			await Awaitable.WaitForSecondsAsync(length);
			sounds.Put(soundPlayer.gameObject);
		}

		[Inject]
		public void Inject(InventoryController inventory)
		{
			_inventory = inventory;
			Refresh(_inventory.Get(itemData));
		}
	}
}