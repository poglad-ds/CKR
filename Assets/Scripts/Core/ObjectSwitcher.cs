using System;
using UnityEngine;

namespace Core
{
	public class ObjectSwitcher : MonoBehaviour
	{
		[SerializeField]
		GameObject[] targets = Array.Empty<GameObject>();

		public void Switch(bool enabled)
		{
			foreach (var target in targets)
				target.SetActive(enabled);
		}
	}
}