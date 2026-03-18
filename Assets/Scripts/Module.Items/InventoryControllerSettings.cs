using System;
using UnityEngine;
using Zenject;
using Core;
using System.Collections.Generic;

namespace Module.Items
{

	[CreateAssetMenu(menuName = "App/Controllers/InventorySettings", fileName = "InventorySettings")]
	public class InventoryControllerSettings : ScriptableObjectInstaller
	{
		[SerializeField]
		SerializableItem[] initialItems = Array.Empty<SerializableItem>();

		public IReadOnlyCollection<SerializableItem> InitialItems => initialItems;

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}
	}
}