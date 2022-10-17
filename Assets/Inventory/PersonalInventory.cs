using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using UnityEngine;

namespace TankGame.Items {

	public class PersonalInventory : MonoBehaviour, IInventory {

		public int slotCount = 4;

		private ItemObject[] slots;

		public int Count {
			get {
				int count = 0;

				for (int i = 0; i < slots.Length; i++) {
					if (slots[i] != null) {
						count++;
					}
				}

				return count;
			}
		}

		public string[] startingItems;

		private void Awake() {
			slots = new ItemObject[slotCount];
		}

		private void Start() {
			foreach (string name in startingItems) {
				ItemObject obj = ItemHolder.Construct(name);

				TryAddItem(obj);
			}
		}

		public List<ItemObject> GetStored() {
			List<ItemObject> list = new List<ItemObject>();

			for (int i = 0; i < slots.Length; i++) {
				if (slots[i] != null) {
					list.Add(slots[i]);
				}
			}

			return list;
		}

		public ItemObject GetAtSlot(int index) {

			return slots[index];
		}

		public bool TryAddItem(ItemObject item) {
			for (int i = 0; i < slots.Length; i++) {
				if (slots[i] == null) {
					slots[i] = item;
					item.transform.SetParent(transform);
					EventBus.Post(new InventoryEvent.ItemAdded(this, item));
					return true;
				}
			}

			return false;
		}

		public bool TryRemoveItem(ItemObject item) {
			for (int i = 0; i < slots.Length; i++) {
				if (slots[i] == item) {
					slots[i] = null;
					EventBus.Post(new InventoryEvent.ItemRemoved(this, item));
					return true;
				}
			}

			return false;
		}

		public bool TransferItem(IInventory targetInv, ItemObject item) {
			for (int i = 0; i < slots.Length; i++) {
				if (slots[i] == item) {
					if (targetInv.TryAddItem(item)) {
						slots[i] = null;
						EventBus.Post(new InventoryEvent.ItemTransfered(this, targetInv, item));
						return true;
					}
					else return false;
				}
			}

			return false;
		}

		public GameObject GetObject() {
			return gameObject;
		}
	}
}