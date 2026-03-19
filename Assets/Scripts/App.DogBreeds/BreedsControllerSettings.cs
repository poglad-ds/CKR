using System;
using System.Linq;
using System.Threading;
using Core.Web;
using UnityEngine;
using Zenject;

namespace App
{
	[CreateAssetMenu(menuName = "App/Controllers/Breeds", fileName = "Breeds")]
	public class BreedsControllerSettings : ScriptableObjectInstaller, IRequestSender<BreedResponse>
	{
		[SerializeField]
		string uri;

		BreedResponse _cachedLatestResponce = new();

		public async Awaitable<BreedResponse> Request(CancellationToken cancellationToken)
		{
			using var request = await WebRequest.CreateGet(uri).Send(WebRequestSendSettings.Default.WithRetryAction(() => Debug.Log("Retry... ")).WithFailedAction(() => Debug.Log("Failed...")), new());

			if (!request)
				return null;

			var resp = request.ParseAsJson<BreedResponse>(_cachedLatestResponce);
			return resp.data;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(this).AsSingle();
		}

	}

	//
	// Было 2 выбора - решил по итогу просто повторить структуру классами вместо затягивания Newtonsoft.Json
	//
	[Serializable]
	public class BreedResponse : IJsonObject
	{
		public BreedData[] data;
	}

	[Serializable]
	public class BreedData
	{
		public string id;
		public string type;
		public BreedAttributes attributes;
		public BreedRelationships relationships;
	}

	[Serializable]
	public class BreedAttributes
	{
		public string name;
		public string description;
		public BreedLifeSpan life;
		public BreedWeightRange male_weight;
		public BreedWeightRange female_weight;
		public bool hypoallergenic;
	}

	[Serializable]
	public class BreedLifeSpan
	{
		public int max;
		public int min;
	}

	[Serializable]
	public class BreedWeightRange
	{
		public int max;
		public int min;

		public float Average => (max + min) / 2f;
	}

	[Serializable]
	public class BreedRelationships
	{
		public GroupRelationship group;
	}

	[Serializable]
	public class GroupRelationship
	{
		public GroupData data;
	}

	[Serializable]
	public class GroupData
	{
		public string id;
		public string type;
	}
}