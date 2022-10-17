using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;

namespace TankGame.UI {

	public class InventorySlot : MonoBehaviour {

		public ItemIcon occupyingItem;

		public PersonalInventory ParentInventory { get; private set; }
		private int index;

		public bool Occupied {
			get {
				return occupyingItem != null;
			}
		}

		void Start() {

		}

		void Update() {

		}

		public void Initialize(PersonalInventory parentInventory, int _index) {
			ParentInventory = parentInventory;
			index = _index;
		}

		public void Initialize(PersonalInventory parentInventory, int _index, ItemIcon _occupyingItem) {
			ParentInventory = parentInventory;
			index = _index;
			occupyingItem = _occupyingItem;
		}

		public bool FillSlot(ItemIcon item) {
			if (Occupied) return false;

			occupyingItem = item;

			item.transform.SetParent(transform);
			item.transform.localPosition = Vector3.zero;

			return true;
		}

		public ItemIcon RemoveItem() {
			ItemIcon item = occupyingItem;

			occupyingItem = null;

			return item;
		}
	}
}