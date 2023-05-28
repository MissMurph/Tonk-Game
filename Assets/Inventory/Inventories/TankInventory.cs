using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Items {

	public class TankInventory : AbstractInventory {

		private Dictionary<Vector2Int, ItemObject> storedItems = new Dictionary<Vector2Int, ItemObject>();

		private Space[,] storageSpaces;

		[SerializeField]
		private Vector2Int gridSize = Vector2Int.one * 5;

		public Vector2Int Size { get { return gridSize; } }

		protected override void Awake() {
			base.Awake();

			storageSpaces = new Space[Size.x, Size.y];

			for (int x = 0; x < Size.x; x++) {
				for (int y = 0; y < Size.y; y++) {
					storageSpaces[x, y] = new Space();
				}
			}
		}

		public override List<ItemObject> GetStored() {
			return new List<ItemObject>(storedItems.Values);
		}

		//This method does not check the cells before placing, make sure cells are valid BEFORE using this function
		//This is because I'd like to come up with a cleaner algorithm than used in TryAddItem function that runs faster
		private void PlaceItemUnsafe (ItemObject item, Vector2Int position) {
			for (int x = 1; x < item.Item.Size.x; x++) {
				for (int y = 1; y < item.Item.Size.y; y++) {
					//if (storageSpaces[x, y].Occupied) return false;

					storageSpaces[x + position.x, y + position.y].OccupyingItem = item;
					
				}
			}

			storedItems.Add(position, item);
			item.transform.SetParent(transform);
			//return true;
		}

		private void RemoveItemUnsafe (ItemObject item, Vector2Int position) {
			for (int x = 1; x < item.Item.Size.x; x++) {
				for (int y = 1; y < item.Item.Size.y; y++) {
					storageSpaces[x + position.x, y + position.y].OccupyingItem = null;
				}
			}

			storedItems.Remove(position);
		}

		public bool ContainsItem (Item item, out ItemObject outObj) {
			foreach (ItemObject itemObj in storedItems.Values) {
				if (itemObj.Item.Name.Equals(item.Name)) {
					outObj = itemObj;
					return true;
				}
			}

			outObj = null;
			return false;
		}

		//If this function returns -1 in either value, it's a null value that hasn't been found.
		public Vector2Int GetItemPos (ItemObject item) {
			foreach (KeyValuePair<Vector2Int, ItemObject> entry in storedItems) {
				if (entry.Value.Equals(item)) {
					return entry.Key;
				}
			}

			return new Vector2Int(-1, -1);
		}

		private Vector2Int FindSpace (ItemObject item) {
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
						//At this point we've identified a successful candidate along the x axis, so we have to verify for the y axis

						int requiredCandidates = item.Item.Size.x * item.Item.Size.y;
						int sucessfulCandidates = currentChain;

						for (int inspectY = 0; inspectY < item.Item.Size.y - 1; inspectY++) {
							for (int inspectX = 0; inspectX < item.Item.Size.x; inspectX++) {
								if (storageSpaces[x - (currentChain - 1) + inspectX, y + inspectY].Occupied) break;

								sucessfulCandidates++;
							}
						}

						if (sucessfulCandidates == requiredCandidates) {
							return new Vector2Int(x - (currentChain - 1), y);
						}
						else currentChain = 0;
					}
				}
			}

			//Return -1,-1 since V2Int cant be null
			return new Vector2Int(-1, -1);
		}

		/*	Interactions	*/

		public Interaction TryEnterItemAtPos (ItemObject item, Character character, Vector2Int pos) {
			for (int x = 1; x < item.Item.Size.x; x++) {
				for (int y = 1; y < item.Item.Size.y; y++) {
					if (storageSpaces[pos.x + x, pos.y + y].Occupied) {
						Debug.LogWarning("Cannot enter requested item! Requested position not available!");
						return null;
					}
				}
			}

			return new TankInvInteraction(item, character, "EnterItem", pos, TankEnterItem, this);
		}

		private InteractionContext<TankInvInteraction> TankEnterItem (TankInvInteraction interaction) {
			Vector2Int pos = interaction.Position;
			ItemObject item = interaction.Item;

			for (int x = 1; x < item.Item.Size.x; x++) {
				for (int y = 1; y < item.Item.Size.y; y++) {
					if (storageSpaces[x + pos.x, y + pos.y].Occupied) return new InteractionContext<TankInvInteraction>(interaction, IPhase.Post, IResult.Fail);
				}
			}

			PlaceItemUnsafe(item, pos);

			return new InteractionContext<TankInvInteraction>(interaction, IPhase.Post, IResult.Success);
		}

		protected override Interaction TryEnterItem (ItemObject item, Character character, string name) {
			//By looping over and checking every single item, we can catch multiple itemObjs of the same Item
			foreach (KeyValuePair<Vector2Int, ItemObject> entry in storedItems) {
				if (entry.Value.Item.Equals(item.Item) && stackDictionary.TryGetValue(entry.Value.Item, out int stackLimit) && entry.Value.StackCount + item.StackCount <= stackLimit) {
					//We'll return a normal Inv interaction for when we can just stack it, otherwise we'll need 2D position
					return new InvInteraction(item, character, EnterItem, this, name);
				}
			}

			//If the above doesn't work we need to find a position and place the item
			Vector2Int pos = FindSpace(item);

			if (pos.x != -1 && pos.y != -1) {
				return TryEnterItemAtPos(item, character, pos);
			}
			
			return null;
		}

		protected override Interaction TryTakeItem (ItemObject item, Character character, string name) {
			foreach (KeyValuePair<Vector2Int, ItemObject> entry in storedItems) {
				if (ReferenceEquals(item, entry.Value) || (entry.Value.Item.Equals(item.Item) && entry.Value.StackCount <= item.StackCount)) {
					return new InvInteraction(item, character, TakeItem, this, name);
				}
			}

			Debug.LogWarning("Cannot take requested item! Not in inventory! Interaction request declined");
			return null;
		}

		private InteractionContext<InvInteraction> TakeItem (InvInteraction interaction) {
			ItemObject item = interaction.Item;

			foreach (KeyValuePair<Vector2Int, ItemObject> entry in storedItems) {
				if (ReferenceEquals(item, entry.Value)) {
					RemoveItemUnsafe(item, entry.Key);
					return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
				}
				else if (entry.Value.Item.Equals(item.Item) && entry.Value.StackCount <= item.StackCount) {
					ItemObject result = entry.Value.TakeFromStack(item.StackCount);

					if (ReferenceEquals(result, entry.Value)) {
						RemoveItemUnsafe(result, entry.Key);
						return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
					}
					else if (result != null) {
						return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
					}
				}
			}

			return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Fail);
		}

		private InteractionContext<InvInteraction> EnterItem (InvInteraction interaction) {
			foreach (KeyValuePair<Vector2Int, ItemObject> entry in storedItems) {
				if (entry.Value.Item.Equals(interaction.Item.Item) && stackDictionary.TryGetValue(entry.Value.Item, out int stackLimit) && entry.Value.StackCount + interaction.Item.StackCount <= stackLimit) {
					entry.Value.AddToStack(interaction.Item);
					return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
				}
			}

			//If the above doesn't work we need to find a position and place the item
			Vector2Int pos = FindSpace(interaction.Item);

			if (pos.x != -1 && pos.y != -1) {
				PlaceItemUnsafe(interaction.Item, pos);

				return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
			}

			return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Fail);
		}

		public override List<AbstractInteractionFactory> GetInteractions () {
			List<AbstractInteractionFactory> output = base.GetInteractions();

			return output;
		}

		//Custom implementation of Interaction for parsing through 2D position data
		public class TankInvInteraction : Interaction<TankInvInteraction> {

			internal ItemObject Item { get; private set; }
			internal Vector2Int Position { get; private set; }

			public TankInvInteraction (ItemObject item, Character character, string name, Vector2Int position, InteractionFunction destination, IInteractable parent) 
				: base(destination, character, name, parent) {
				Item = item;
				Position = position;
			}
		}

		//The 2D inventory is made up of a table of these structs, acting as each cell in the grid
		private struct Space {
			public ItemObject OccupyingItem;

			public bool Occupied {
				get
				{
					return OccupyingItem != null;
				}
			}
		}
	}
}