using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App
{
	public class BreedButtonView : MonoBehaviour
	{
		[SerializeField]
		Button requestDetailButton;

		[SerializeField]
		TMP_Text breedNameText;

		BreedView _view;

		BreedData _cached;

		void Start()
		{
			requestDetailButton?.onClick.AddListener(Open);
		}

		void OnEnable()
		{
			if (breedNameText)
				breedNameText.text = string.Empty;
		}

		public void Init(BreedData data)
		{
			breedNameText.text = data.attributes.name;
			_cached = data;
		}

		void Open()
		{
			if (_cached != null)
				_view.Init(_cached);
		}

		[Inject]
		public void Inject(BreedView view)
		{
			_view = view;
		}
	}
}