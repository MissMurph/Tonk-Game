using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Items {

	public class TankInventory : MonoBehaviour, IInventory {

		private List<ItemObject> storedItems = new List<ItemObject>();

		public List<ItemObject> GetStored() {
			return new List<ItemObject>(storedItems);
		}

		public bool TransferItem(IInventory targetInv, ItemObject item) {
			return false;
		}

		public bool TryAddItem(ItemObject item) {
			return false;
		}

		public bool TryRemoveItem(ItemObject item) {
			return false;
		}

		public GameObject GetObject() {
			return gameObject;
		}
	}
}