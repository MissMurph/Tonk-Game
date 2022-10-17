using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame.Items {

	public class ItemHolder : MonoBehaviour {

		private static ItemHolder instance;

		[SerializeField]
		private GameObject itemPrefab;

		public ItemEntry[] entries;

		private static Dictionary<string, AbstractItem> registeredItems = new Dictionary<string, AbstractItem>();

		//public static AbstractItem TEST_ITEM = RegisterItem<GenericItem>("test", (name) => new GenericItem(name, new Vector2Int(2, 2)));

		private void Awake() {
			instance = this;

			foreach (ItemEntry entry in entries) {
				//

				RegisterItem<GenericItem>(entry.Name, (name) => new GenericItem(name, entry.Size, entry.Icon));
			}
		}

		public static ItemObject Construct(string name) {
			if (registeredItems.TryGetValue(name, out AbstractItem item)) {
				ItemObject itemObject = Instantiate(instance.itemPrefab, instance.transform).GetComponent<ItemObject>();
				itemObject.Initialize(item);
				return itemObject;
			}

			return null;
		}

		private delegate T ItemConstructor<T>(string name);

		private static AbstractItem RegisterItem<T>(string name, ItemConstructor<T> supplier) where T : AbstractItem {
			T item = supplier.Invoke(name);
			registeredItems.TryAdd(name, item);
			return item;
		}
	}

	[Serializable]
	public class ItemEntry {
		public string Name;
		public Vector2Int Size;
		public Sprite Icon;
	}
}