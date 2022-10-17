using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame.UI {

	public class ItemIcon : MonoBehaviour {

		public ItemObject Item { get; private set; }

		public InventorySlot ParentSlot {
			get {
				return transform.parent.GetComponent<InventorySlot>();
			}
		}

		[SerializeField]
		private Image image;

		private void Awake() {
			image = GetComponent<Image>();
		}

		public void Initialize(ItemObject _parentItem) {
			Item = _parentItem;
			image.sprite = Item.Item.Icon;
		}

		public void OnClick() {

		}
	}
}