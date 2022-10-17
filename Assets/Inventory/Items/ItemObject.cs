using UnityEngine;

namespace TankGame.Items {

	public class ItemObject : MonoBehaviour {

		public AbstractItem Item { get; private set; }

		public void Initialize(AbstractItem _item) {
			Item = _item;
		}
	}
}