using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;

namespace TankGame.UI {

	public class InventorySlot : MonoBehaviour {

		protected ItemIcon occupyingItem;

		public IInventory ParentInventory { get; private set; }

		public ItemIcon OccupyingItem {
			get { 
				return occupyingItem; 
			}
		}

		public bool Occupied {
			get {
				return occupyingItem != null;
			}
		}

		void Start() {

		}

		void Update() {

		}

		public virtual void Initialize(IInventory parentInventory) {
			ParentInventory = parentInventory;
		}

		public virtual void Initialize(IInventory parentInventory, ItemIcon _occupyingItem) {
			ParentInventory = parentInventory;
			occupyingItem = _occupyingItem;
		}

		public virtual bool FillSlot(ItemIcon item) {
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