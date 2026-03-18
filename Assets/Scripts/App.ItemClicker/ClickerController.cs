using Module.Items;
using UnityEngine;
using Zenject;
using System;
using System.Threading;

namespace App
{
	[CreateAssetMenu(menuName = "App/Controllers/Clicker", fileName = "Clicker")]
	public class ClickerController : ScriptableObjectInstaller
	{
		/// <summary>
		/// Clicker manually clicked for score.
		/// Well, Inventory.Updated is for more global item changer, that not our case 
		/// </summary>
		public event Action Clicked;

		[SerializeField]
		ItemData energy;

		[SerializeField]
		ItemData score;

		[Space(5)]
		[Header("Enegy regeneration")]
		[SerializeField]
		SerializableItem energyCounter;

		[SerializeField]
		float energyRecoupDelayInSeconds;

		[SerializeField]
		long energyCap;

		[Space(5)]
		[SerializeField]
		float scoreAutoclickerDelayInSeconds;

		InventoryController _inventory;
		Item _cachedEnergyCounter;

		Item _cachedScoreCounter;
		Item _cachedScoreEnergyCounter;
		CancellationToken _backgroundCancellationToken;

		[Inject]
		public void Initialize(InventoryController inventory)
		{
			_cachedEnergyCounter = energyCounter.AsItem();
			_cachedScoreCounter = new() { Data = score, Count = 1 };
			_cachedScoreEnergyCounter = new() { Data = energy, Count = -1 };

			_inventory = inventory;

			if (!_inventory)
				return;

			_inventory.Updated += OnInventoryUpdate;

			_backgroundCancellationToken = new();
			_ = EnergyRecuperator(_backgroundCancellationToken);
			_ = ScoreClicker(_backgroundCancellationToken);
		}

		public void TryClick()
		{
			if (!_inventory)
				return;

			if (_inventory.TryChange(_cachedScoreCounter))
				_inventory.TryChange(_cachedScoreEnergyCounter);
		}

		async Awaitable EnergyRecuperator(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await Awaitable.WaitForSecondsAsync(energyRecoupDelayInSeconds, cancellationToken);

				if (!_inventory)
					continue;

				var diff = energyCap - _inventory.Count(energy);

				if (diff <= 0)
					continue;

				if (diff >= _cachedEnergyCounter.Count)
				{
					_inventory.TryChange(_cachedEnergyCounter);
					continue;
				}

				_inventory.TryChange(new Item() { Data = _cachedEnergyCounter.Data, Count = diff });
			}
		}

		async Awaitable ScoreClicker(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await Awaitable.WaitForSecondsAsync(scoreAutoclickerDelayInSeconds, cancellationToken);
				TryClick();
			}
		}


		private void OnInventoryUpdate(Item item)
		{
			if (item.Data.Equals(score))
				return;

		}

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}
	}
}