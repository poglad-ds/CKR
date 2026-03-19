using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace App
{
	public class BreedView : DefaultWindow<BreedView>, IWindow
	{
		[SerializeField]
		TMP_Text breedNameText;

		[SerializeField]
		TMP_Text breedDescriptonText;

		void OnEnable()
		{
			if (breedNameText)
				breedNameText.text = string.Empty;

			if (breedDescriptonText)
				breedDescriptonText.text = string.Empty;
		}

		public void Init(BreedData data)
		{
			_windowController.Open(this);

			breedNameText.text = data.attributes.name;
			breedDescriptonText.text = data.attributes.description;
		}

		[Inject]
		public void Inject(WindowController controller)
		{
			_windowController = controller;
		}
	}
}