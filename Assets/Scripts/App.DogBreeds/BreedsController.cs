using System;
using System.Linq;
using Core.Web;
using UnityEngine;
using Zenject;

namespace App
{
	[CreateAssetMenu(menuName = "App/Controllers/BreedsController", fileName = "BreedsController")]
	public class BreedsController : ScriptableObjectInstaller
	{
		[SerializeField]
		string uri;

		BreedResponse _cachedLatestResponce = new();

		public async Awaitable<BreedData> RequestData()
		{
			using var request = await WebRequest.CreateGet(uri).Send(WebRequestSendSettings.Default.WithRetryAction(() => Debug.Log("Retry... ")));

			if (!request)
				return null;

			var resp = request.Parse<BreedResponse>(_cachedLatestResponce);

			return resp.data.data.FirstOrDefault();
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