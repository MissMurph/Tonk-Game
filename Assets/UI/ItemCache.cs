using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.UI {

	public class ItemCache : InventoryElement {

		private SupplyCache linkedInv;

		public override AbstractInventory GetLinked () {
			return linkedInv;
		}

		public override bool TryEnterItem (ItemIcon item, InventorySlot slot) {
			if (!slot.Occupied && item.ParentSlot.ParentInventory.GetLinked().TryGetComponent(out Character character)) {
				AbstractInteraction interaction = linkedInv.TryEnterItemUI(item.Item, character);
			}


			return false;
		}

		public override bool TryTakeItem (ItemIcon item) {
			throw new System.NotImplementedException();
		}
	}
}