using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;

namespace TankGame.Events {

	public class InventoryEvent : AbstractEvent {

		public IInventory Inventory { get; private set; }
		public ItemObject Item { get; private set; }

		protected InventoryEvent(IInventory inventory, ItemObject item, string name) : base(name) {
			Inventory = inventory;
			Item = item;
		}

		public class ItemAdded : InventoryEvent {
			public ItemAdded(IInventory inventory, ItemObject item) : base(inventory, item, "item_added") {
			}
		}

		public class ItemRemoved : InventoryEvent {
			public ItemRemoved(IInventory inventory, ItemObject item) : base(inventory, item, "item_removed") {
			}
		}

		public class ItemTransfered : InventoryEvent {
			public IInventory Target { get; private set; }

			public ItemTransfered(IInventory origin, IInventory target, ItemObject item) : base(origin, item, "item_removed") {
				Target = target;
			}
		}
	}
}