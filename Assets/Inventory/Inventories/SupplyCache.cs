using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Items {

	public class SupplyCache : AbstractInventory {

		[SerializeField]
		private int slotCount = 3;

		private ItemObject[] slots;

		private List<ItemObject> itemList = new List<ItemObject>();

		[SerializeField]
		private int searchWorkRequired = 15;

		private int searchWorkDone = 0;

		public float SearchProgress {
			get {
				return (float)searchWorkDone / searchWorkRequired;
			}
		}

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

		[SerializeField]
		private string[] startingItems;

		protected override void Awake () {
			base.Awake();

			slots = new ItemObject[slotCount];
		}

		private void Start () {
			foreach (string name in startingItems) {
				ItemObject obj = Items.Construct(name);

				slots[0] = obj;
				itemList.Add(obj);
			}

			manager.AddListener<InvInteraction>("TakeItem", ListenerTake);
			manager.AddListener<InvInteraction>("EnterItem", ListenerEnter);
		}

		public override List<ItemObject> GetStored () {
			List<ItemObject> list = new List<ItemObject>();

			for (int i = 0; i < slots.Length; i++) {
				if (slots[i] != null) {
					list.Add(slots[i]);
				}
			}

			return list;
		}

		public override List<AbstractInteractionFactory> GetInteractions () {
			List<AbstractInteractionFactory> output = new List<AbstractInteractionFactory>();
			output.Add(new GenericInteractionFactory("Search", TrySearch));
			
			output.AddRange(base.GetInteractions());

			return output;
		}


		/*	Interaction Requisition	*/

		protected override AbstractInteraction TryEnterItem (ItemObject item, Character character, string name) {
			if (searchWorkDone < searchWorkRequired) {
				Debug.LogWarning("Cache not searched! Cannot transfer items until Search is complete!");
				return null;
			}

			if (Count < slotCount) {
				return new InvInteraction(item, character, EnterItem, this, name);
			}

			Debug.LogWarning("Cannot enter requested item! Not enough space in inventory! Interaction request declined");
			return null;
		}

		protected override AbstractInteraction TryTakeItem (ItemObject item, Character character, string name) {
			if (searchWorkDone < searchWorkRequired) {
				Debug.LogWarning("Cache not searched! Cannot transfer items until Search is complete!");
				return null;
			}

			if (GetStored().Contains(item)) {
				return new InvInteraction(item, character, TakeItem, this, name);
			}

			Debug.LogWarning("Cannot take requested item! Not in inventory! Interaction request declined");
			return null;
		}

		protected GenericInteraction TrySearch (Character character, string name) {
			if (searchWorkDone < searchWorkRequired) {
				return new GenericInteraction(Search, character, name, this);
			}

			Debug.LogWarning("Cannot Search Cache! Cache has already been searched!");
			return null;
		}


		/*	Interaction Acting Functions	*/

		//Interaction Function for taking an item from this inventory
		private InteractionContext<InvInteraction> TakeItem (InvInteraction interaction) {
			ItemObject item = interaction.Item;

			for (int i = 0; i < slots.Length; i++) {
				if (ReferenceEquals(slots[i], item)) {
					slots[i] = null;
					itemList.Remove(item);
					return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
				}
				//If we cannot locate the same reference, we'll need to find an ItemObject that can legally supply the same item Type & Requested Stack Amount
				else if (slots[i].Item.Equals(item.Item) && slots[i].StackCount >= item.StackCount) {
					ItemObject result = slots[i].TakeFromStack(item.StackCount);

					if (result.Equals(slots[i])) {
						slots[i] = null;
						itemList.Remove(result);
						return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
					}
					else if (result != null) {
						return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
					}
				}
			}

			return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Fail);
		}

		//Interaction Function for entering an item into this inventory
		private InteractionContext<InvInteraction> EnterItem (InvInteraction interaction) {
			ItemObject item = interaction.Item;

			foreach (ItemObject obj in itemList) {
				if (ReferenceEquals(obj.Item, item.Item) && stackDictionary.TryGetValue(item.Item, out int stackLimit) && obj.StackCount + item.StackCount <= stackLimit) {
					obj.AddToStack(item);
					//EventBus.Post(new InventoryEvent.ItemAdded(this, item));
					return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
				}
			}

			for (int i = 0; i < slots.Length; i++) {
				if (slots[i] == null) {
					slots[i] = item;
					item.transform.SetParent(transform);
					itemList.Add(item);
					//EventBus.Post(new InventoryEvent.ItemAdded(this, item));
					return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Success);
				}
			}

			return new InteractionContext<InvInteraction>(interaction, IPhase.Post, IResult.Fail);
		}

		private InteractionContext<GenericInteraction> Search (GenericInteraction interaction) {
			searchWorkDone++;

			if (searchWorkDone >= searchWorkRequired) {
				return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Success);
			}

			return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Continue);
		}

		/*	Interaction Listeners	*/

		//When an item is taken from another inventory we need to update the slot and set the parent of the ItemObject
		private void ListenerTake (InteractionContext<InvInteraction> context) {
			if (context.Phase.Equals(IPhase.Pre)) {
				foreach (ItemObject obj in itemList) {
					if (ReferenceEquals(obj.Item, context.Interaction.Item.Item) && stackDictionary.TryGetValue(context.Interaction.Item.Item, out int stackLimit) && obj.StackCount < stackLimit) {
						return;
					}
				}

				for (int i = 0; i < slots.Length; i++) {
					if (slots[i] == null) {
						return;
					}
				}

				//We do another check to verify inventory changes can be made since the initial check when Interaction was constructed
				//If neither the above checks succeed we need to cancel the interaction
				context.Cancel();
			}

			if (context.Phase.Equals(IPhase.Post) && context.Result.Equals(IResult.Success)) {
				ItemObject item = context.Interaction.Item;

				foreach (ItemObject obj in itemList) {
					if (ReferenceEquals(obj.Item, item.Item) && stackDictionary.TryGetValue(item.Item, out int stackLimit) && obj.StackCount < stackLimit) {
						item = obj.AddToStack(item);
						return;
					}
				}

				for (int i = 0; i < slots.Length; i++) {
					if (slots[i] == null) {
						slots[i] = item;
						item.transform.SetParent(transform);
						itemList.Add(item);
						return;
					}
				}
			}
		}

		//When another inventory receives an item from this one, we need to update the slot
		private void ListenerEnter (InteractionContext<InvInteraction> context) {
			if (context.Phase.Equals(IPhase.Pre)) {
				foreach (ItemObject obj in itemList) {
					if (ReferenceEquals(obj.Item, context.Interaction.Item.Item)) {
						return;
					}
				}

				//We do another check to verify inventory changes can be made since the initial check when Interaction was constructed
				//If neither the above checks succeed we need to cancel the interaction
				context.Cancel();
			}

			if (context.Phase.Equals(IPhase.Post) && context.Result.Equals(IResult.Success)) {
				ItemObject item = context.Interaction.Item;

				for (int i = 0; i < slots.Length; i++) {
					if (ReferenceEquals(slots[i], item)) {
						slots[i] = null;
						itemList.Remove(item);
						return;
					}
				}
			}
		}
	}
}