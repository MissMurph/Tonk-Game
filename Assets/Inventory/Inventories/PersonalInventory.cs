using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using TankGame.Capabilities;
using TankGame.Events;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Items {

	public class PersonalInventory : AbstractInventory {

		public int slotCount = 4;

		private ItemObject[] slots;

		private List<ItemObject> stored = new List<ItemObject>();

		private Dictionary<Interactionlet, Reservation> addReservations;
		private Dictionary<Interactionlet, Reservation> removeReservations;

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

		protected override void Awake() {
			base.Awake();

			slots = new ItemObject[slotCount];
			addReservations = new Dictionary<Interactionlet, Reservation>();
			removeReservations = new Dictionary<Interactionlet, Reservation>();
		}

		private void Start() {
			foreach (string name in startingItems) {
				ItemObject obj = Items.Construct(name, transform);

				slots[0] = obj;
				stored.Add(obj);
			}
		}

		public override List<ItemObject> GetStored() {
			List<ItemObject> list = new List<ItemObject>();

			for (int i = 0; i < slots.Length; i++) {
				if (slots[i] != null) {
					list.Add(slots[i]);
				}
			}

			return list;
		}

		public ItemObject GetAtSlot(int index) {
			return slots[index];
		}

		//If -1 is the result the item could not be found
		public int GetItemIndex (ItemObject item) {
			for (int i = 0; i < slotCount; i++) {
				if (ReferenceEquals(slots[i], item)) {
					return i;
				}
			}

			Debug.LogWarning("Item Not found! Index returned as -1");
			return -1;
		}

		private bool IsLegalToAdd (ItemObject item, int slotIndex) {
			stackDictionary.TryGetValue(item.Item, out int limit);

			if (slots[slotIndex] == null && item.StackCount <= limit) return true;

			if (slots[slotIndex].Item.Equals(item.Item)) {
				int reservedStacking = 0;

				foreach (KeyValuePair<Interactionlet, Reservation> entry in addReservations) {
					if (entry.Value.slot == slotIndex && entry.Value.item.Equals(item.Item)) {
						reservedStacking += entry.Value.stack;
					}
				}

				if (slots[slotIndex].StackCount + item.StackCount + reservedStacking <= limit) {
					return true;
				}
			}

			return false;
		}

		/*	Requisition	*/

		/**Read Data:
		 * item:name			string name of desired item
		 * item:amount			int amount of desired item
		 * item:concrete		ItemObject of desired item
		 * slot:number			int number of desired slot
		 */
		protected override IResult RequestAdd (Actor actor, Interactionlet packet) {
			if (packet.Step.Equals(IStep.Request) && packet.Data.TryGetValue("item:concrete", out ICapability itemObj)) {
				ItemObject concrete = (itemObj as ItemCapability).Get();

				for (int i = 0; i < slotCount; i++) {
					if (packet.Data.TryGetValue("slot:number", out ICapability slotNumber) && i != slotNumber.Int()) continue;

					if (IsLegalToAdd(concrete, i)) {
						addReservations.Add(packet, new Reservation { slot = i, item = concrete.Item, stack = concrete.StackCount});
						return IResult.Success;
					}
				}
			}

			return IResult.Fail;
		}

		protected override IResult RequestRemove (Actor actor, Interactionlet packet) {
			if (packet.Step.Equals(IStep.Request) && packet.Data.TryGetValue("item:name", out ICapability name) && packet.Data.TryGetValue("item:amount", out ICapability amount)) {
				foreach (ItemObject item in stored) {
					if (item.Item.Name.Equals(name.String()) && item.StackCount >= amount.Int()) {
						packet.Data.Add("item:concrete", new ItemCapability(item));
						removeReservations.Add(packet, new Reservation { slot = GetItemIndex(item), item = item.Item, stack = amount.Int()});
						return IResult.Success;
					}
				}
			}

			return IResult.Fail;
		}

		/*	Acting	*/

		protected override IResult ActAdd (Actor actor, Interactionlet packet) {
			//By comparing the Interactionlet against the reservation, we can implicitly confirm from the request stage that this interaction is possible and conditions
			//haven't changed since the request was made
			if (packet.Step.Equals(IStep.Act) && packet.Data.TryGetValue("item:concrete", out ICapability itemObj) && addReservations.ContainsKey(packet)) {
				ItemObject concrete = (itemObj as ItemCapability).Get();

				for (int i = 0; i < slotCount; i++) {
					if (packet.Data.TryGetValue("slot:number", out ICapability slotNumber) && i != slotNumber.Int()) continue;

					if (slots[i] is null) {
						slots[i] = concrete;
						concrete.transform.SetParent(transform);
						stored.Add(concrete);
					}
					else {
						slots[i].AddToStack(concrete);
					}

					addReservations.Remove(packet);
					return IResult.Success;
				}
			}

			return IResult.Fail;
		}

		protected override IResult ActRemove (Actor actor, Interactionlet packet) {
			if (packet.Step.Equals(IStep.Act) && packet.Data.TryGetValue("item:concrete", out ICapability itemObj) && addReservations.ContainsKey(packet)) {

				for (int i = 0; i < slotCount; i++) {
					if (packet.Data.TryGetValue("slot:number", out ICapability slotNumber) && i != slotNumber.Int()) continue;

					removeReservations.TryGetValue(packet, out Reservation reservation);

					if (slots[i].StackCount > reservation.stack) {
						ItemObject newObj = slots[i].TakeFromStack(reservation.stack);
						packet.Data["item:concrete"] = new ItemCapability(newObj);
						removeReservations.Remove(packet);
						
					}
					else {
						packet.Data["item:concrete"] = new ItemCapability(slots[i]);
						slots[i] = null;
						stored.Remove(slots[i]);
					}

					removeReservations.Remove(packet);
					return IResult.Success;
				}
			}

			return IResult.Fail;
		}

		/*	Listening	*/

		//This is listening for if we're adding one of our items to another
		protected override void ListenAdd (Interactionlet packet) {
			if (packet.Step.Equals(IStep.Act) && packet.Result.Equals(IResult.Success) && packet.Data.TryGetValue("item:concrete", out ICapability itemObj)) {
				ItemObject concrete = (itemObj as ItemCapability).Get();

			}
		}

		//This is listening if we're removing an item from another inventory
		protected override void ListenRemove (Interactionlet packet) {
			throw new System.NotImplementedException();
		}

		private struct Reservation {
			internal int slot;
			internal Item item;
			internal int stack;
		}
	}
}