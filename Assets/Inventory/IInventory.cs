using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using UnityEngine;

namespace TankGame.Items {

	public interface IInventory {
		List<ItemObject> GetStored();
		bool TryAddItem(ItemObject item);
		bool TryRemoveItem(ItemObject item);
		bool TransferItem(IInventory targetInv, ItemObject item);
		GameObject GetObject();
	}
}