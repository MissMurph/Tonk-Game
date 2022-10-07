using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}