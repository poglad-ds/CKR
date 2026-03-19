using System.Threading.Tasks;
using App;
using Core;
using UnityEngine;
using Zenject;

public class BreedPool : PooledList<BreedButtonView>
{
	BreedView _view;

	[Inject]
	public void Inject(BreedView view)
	{
		_view = view;
	}

	public override async Awaitable<BreedButtonView> GetAsComponent()
	{
		var comp = await base.GetAsComponent();
		comp.Inject(_view);

		return comp;
	}
}