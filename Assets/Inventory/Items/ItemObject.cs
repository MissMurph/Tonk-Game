using UnityEngine;

namespace TankGame.Items {

	public class ItemObject : MonoBehaviour {

		public AbstractItem Item { get; private set; }

		public int StackCount { get; private set; }

		public AbstractInventory ParentInventory {
			get {
				return GetComponentInParent<AbstractInventory>();
			}
		}

		public void Initialize(AbstractItem _item) {
			Initialize(_item, 0);
		}

		public void Initialize (AbstractItem _item, int _stackCount) {
			Item = _item;
			StackCount = _stackCount;
		}

		public ItemObject AddToStack (ItemObject newObj) {
			if (newObj.Item.Equals(Item)) {
				StackCount += newObj.StackCount;
				Destroy(newObj.gameObject);
				return this;
			}

			Debug.LogError("Could not add to stack, submitted Item returned");
			return newObj;
		}

		public ItemObject TakeFromStack (int amount) {
			if (amount < StackCount) {
				StackCount -= amount;

				ItemObject newObj = ItemHolder.Construct(Item.Name, amount);

				return newObj;
			}

			if (amount == StackCount) return this;

			Debug.LogError("Requested Stack could not be fulfilled! Null object returned");
			return null;
		}
	}
}