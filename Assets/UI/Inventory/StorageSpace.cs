using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;

namespace TankGame.UI {

	public class StorageSpace : InventorySlot {

		private TankInventory castedParentInv;

		public Vector2Int Position;

		/*private void Awake() {
			castedParentInv = (TankInventory) ParentInventory;
		}

		public override bool FillSlot(ItemIcon item) {
			for (int x = 0; x < item.Item.Item.Size.x; x++) {
				for (int y = 0; y < item.Item.Item.Size.y; y++) {
					//if (ParentInventory.) {

					//}
				}
			}

			return false;
		}*/
	}
}