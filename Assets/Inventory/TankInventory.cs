using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using UnityEngine;

namespace TankGame.Items {

	public class TankInventory : MonoBehaviour, IInventory {

		private Dictionary<Vector2Int, ItemObject> storedItems = new Dictionary<Vector2Int, ItemObject>();

		private Space[,] storageSpaces;

		private Dictionary<string, int> stackDictionary = new Dictionary<string, int>();

		public StackEntry[] stackEntries;

		[SerializeField]
		private Vector2Int gridSize = Vector2Int.one * 5;

		public Vector2Int Size { get { return gridSize; } }

		private void Awake() {
			storageSpaces = new Space[Size.x, Size.y];

			foreach (StackEntry entry in stackEntries) {
				stackDictionary.TryAdd(entry.itemName, entry.stackLimit);
			}

			for (int x = 0; x < Size.x; x++) {
				for (int y = 0; y < Size.y; y++) {
					storageSpaces[x, y] = new Space();
				}
			}
		}

		public List<ItemObject> GetStored() {
			return new List<ItemObject>(storedItems.Values);
		}

		public bool TransferItem(IInventory targetInv, ItemObject item) {
			if (storedItems.ContainsValue(item) && targetInv.TryAddItem(item)) {
				if (!TryRemoveItem(item)) {
					Debug.Log("FAILED TO REMOVE ITEM MID TRANSFER, CHECK TARGET INV FOR DUPLICATE ITEM");
				}

				EventBus.Post(new InventoryEvent.ItemTransfered(this, targetInv, item));
				return true;
			}

			return false;
		}

		public bool TryAddItem(ItemObject item) {
			bool success = false;

			if (ContainsItem(item.Item, out ItemObject obj)
				&& stackDictionary.TryGetValue(item.Item.Name, out int stackLimit)
				&& obj.StackCount + item.StackCount < stackLimit) {

				obj.Stack(item);
				success = true;
			}
			else {
				//If there's no existing object to stack with, we need to search available spaces to determine where to stack
				//Here we're evaluating the highest number of spaces in a chain that are available in each column/row

				int[] rowTotals = new int[Size.y];
				int[] columnTotals = new int[Size.x];

				for (int y = 0; y < Size.y; y++) {
					int highestChain = 0;
					int currentChain = 0;

					for (int x = 0; x < Size.x; x++) {
						if (storageSpaces[x, y].Occupied) {
							currentChain = 0;
							continue;
						}
						else {
							currentChain++;

							if (currentChain > highestChain) highestChain = currentChain;
						}
					}

					rowTotals[y] = highestChain;
				}

				for (int x = 0; x < Size.x; x++) {
					int highestChain = 0;
					int currentChain = 0;

					for (int y = 0; y < Size.y; y++) {
						if (storageSpaces[x, y].Occupied) {
							currentChain = 0;
							continue;
						}
						else {
							currentChain++;

							if (currentChain > highestChain) highestChain = currentChain;
						}
					}

					columnTotals[x] = highestChain;
				}

				//Here we need to eliminate all totals that don't fit

				List<int> successfulRows = new List<int>();
				List<int> successfulColumns = new List<int>();

				for (int i = 0; i < rowTotals.Length; i++) {
					if (rowTotals[i] >= item.Item.Size.x) successfulRows.Add(i);
				}

				for (int i = 0; i < columnTotals.Length; i++) {
					if (rowTotals[i] >= item.Item.Size.y) successfulColumns.Add(i);
				}

				foreach (int y in successfulRows) {
					int currentChain = 0;

					for (int x = 0; x < Size.x; x++) {
						if (!storageSpaces[x, y].Occupied) currentChain++;
						else currentChain = 0;

						if (currentChain == item.Item.Size.x) {
							for (int inspectX = 0; inspectX < item.Item.Size.x; inspectX++) {
								if (storageSpaces[x - currentChain + inspectX, y + 1].Occupied) break;

								PlaceItemUnsafe(item, new Vector2Int(x - currentChain + inspectX, y + 1));
								success = true;
							}

							for (int inspectX = 0; inspectX < item.Item.Size.x; inspectX++) {
								if (storageSpaces[x - currentChain + inspectX, y - 1].Occupied) {
									break;
								}

								PlaceItemUnsafe(item, new Vector2Int(x - currentChain + inspectX, y - 1));
								success = true;
							}
						}
					}
				}
			}

			if (success) EventBus.Post(new InventoryEvent.ItemAdded(this, item));
			return success;
		}

		public bool TryRemoveItem(ItemObject item) {
			foreach (KeyValuePair<Vector2Int, ItemObject> entry in storedItems) {
				if (entry.Value.Equals(item)) {
					storedItems.Remove(entry.Key);

					for (int x = 0; x < item.Item.Size.x; x++) {
						for (int y = 0; y < item.Item.Size.y; y++) {
							storageSpaces[x + entry.Key.x, y + entry.Key.y].OccupyingItem = null;
						}
					}

					EventBus.Post(new InventoryEvent.ItemRemoved(this, item));
					return true;
				}
			}

			return false;
		}

		//This method does not check the cells before placing, make sure cells are valid BEFORE using this function
		private void PlaceItemUnsafe (ItemObject item, Vector2Int position) {
			for (int x = 0; x < item.Item.Size.x; x++) {
				for (int y = 0; y < item.Item.Size.y; y++) {
					//if (storageSpaces[x, y].Occupied) return false;

					storageSpaces[x + position.x, y + position.y].OccupyingItem = item;
					
				}
			}

			storedItems.Add(position, item);
			item.transform.SetParent(transform);
			//return true;
		}

		private bool ContainsItem (AbstractItem item, out ItemObject outObj) {
			foreach (ItemObject itemObj in storedItems.Values) {
				if (itemObj.Item.Name.Equals(item.Name)) {
					outObj = itemObj;
					return true;
				}
			}

			outObj = null;
			return false;
		}

		public GameObject GetObject() {
			return gameObject;
		}

		private struct Space {
			public ItemObject OccupyingItem;

			public bool Occupied {
				get {
					return OccupyingItem != null;
				}
			}
		}
	}

	[System.Serializable]
	public class StackEntry {
		public string itemName;
		public int stackLimit;
	}
}