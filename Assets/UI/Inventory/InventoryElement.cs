using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;

namespace TankGame.UI {

	public abstract class InventoryElement : LinkedElement<AbstractInventory> {

		public abstract bool TryEnterItem (ItemIcon item, InventorySlot slot);
		public abstract bool TryTakeItem (ItemIcon item);
	}
}