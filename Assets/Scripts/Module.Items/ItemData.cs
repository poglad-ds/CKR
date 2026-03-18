using UnityEngine;
using Core;
using UnityEditor;

namespace Module.Items
{
	public interface IItemData
	{
		public Ref<Sprite> Icon { get; }

		public string Name { get; }
	}


	[CreateAssetMenu(menuName = "App/New Item", fileName = "NewItem")]
	public class ItemData : ScriptableObject, IItemData
	{
		[SerializeField]
		string itemName;

		[SerializeField]
		Ref<Sprite> sprite;

		public Ref<Sprite> Icon => sprite;

		public string Name => itemName;

		void OnValidate()
		{
#if UNITY_EDITOR
			if (AssetDatabase.IsAssetImportWorkerProcess())
				return;

			name = Name;
			AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), Name);
			EditorUtility.SetDirty(this);
#endif
		}
	}
}