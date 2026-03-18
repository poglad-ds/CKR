using System.Linq;
using Core;
using UnityEngine;
using Zenject;

namespace Module.Items
{
	public class ItemView : MonoBehaviour
	{
		[SerializeField]
		RefImageView refView;

		InventoryController _inventory;

		[Inject]
		public void Initialize(InventoryController inventory)
		{
			if (!inventory)
				return;

			if (!refView)
				return;

			_ = refView.Pass(inventory.Items.FirstOrDefault().Data.Icon);
		}
	}
}