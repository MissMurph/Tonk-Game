using UnityEngine;

namespace TankGame.Items {

	public class ItemObject : MonoBehaviour {

		public AbstractItem Item { get; private set; }

		public int StackCount { get; private set; }

		public IInventory ParentInventory {
			get {
				return GetComponentInParent<IInventory>();
			}
		}

		public void Initialize(AbstractItem _item) {
			Item = _item;
		}

		public ItemObject Stack (ItemObject newObj) {
			if (newObj.Item.Equals(Item)) {
				StackCount += newObj.StackCount;
				Destroy(newObj.gameObject);
			}

			return this;
		}
	}
}