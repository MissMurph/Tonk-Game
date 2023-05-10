using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame.Items {

	public class Items : SerializedMonoBehaviour {

		private static Items instance;

		[SerializeField]
		private GameObject itemPrefab;

		[NonSerialized]
		[OdinSerialize]
		public Item[] entries;

		private Dictionary<string, Item> registeredItems = new Dictionary<string, Item>();

		private void Awake() {
			instance = this;

			foreach (Item entry in entries) {
				Register(entry);
			}
		}

		private void OnDestroy () {
			instance = null;
		}

		public static Item GetItem (string key) {
			if (instance.registeredItems.TryGetValue(key, out Item result)) {
				return result;
			}

			Debug.LogException(new ArgumentNullException("No Registry Value found!"));
			return null;
		}

		public static ItemObject Construct(string name, int stackAmount, Transform owner) {
			if (instance.registeredItems.TryGetValue(name, out Item item)) {
				ItemObject itemObject = Instantiate(instance.itemPrefab, instance.transform).GetComponent<ItemObject>();
				itemObject.Initialize(item);
				itemObject.transform.SetParent(owner);
				itemObject.transform.localPosition = Vector3.zero;
				return itemObject;
			}

			return null;
		}

		public static ItemObject Construct (string name, Transform owner) {
			return Construct(name, 1, owner);
		}

		private delegate T ItemConstructor<T>(string name);

		private static Item Register (Item entry) {
			instance.registeredItems.TryAdd(entry.Name, entry);
			return entry;
		}
	}
}