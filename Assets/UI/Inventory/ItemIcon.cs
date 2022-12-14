using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Players;
using UnityEngine.EventSystems;
using TankGame.Units;
using TankGame.Units.Commands;
using static TankGame.Units.Commands.Command;

namespace TankGame.UI {

	public class ItemIcon : LinkedElement<ItemObject> {

		public ItemObject Item { get; private set; }

		private bool selected = false;

		private RectTransform rectTransform;

		public InventorySlot ParentSlot { get; private set; }

		[SerializeField]
		private Image image;

		protected override void Awake() {
			base.Awake();
			image = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();
		}

		private void Update() {
			if (selected) {
				transform.position = Player.Controller.MousePos;
				
			}
			else {
				rectTransform.anchoredPosition = Vector2.zero;
			}
		}

		public void Initialize(ItemObject _parentItem, InventorySlot _slot) {
			Item = _parentItem;
			ParentSlot = _slot;
			image.sprite = Item.Item.Icon;
		}

		public void OnClick () {
			selected = true;
			transform.SetParent(Player.UI.GRaycaster.transform);
			transform.SetAsLastSibling();
		}

		public void OnRelease () {
			List<RaycastResult> hits = Player.UI.GetClickedObjects();

			foreach (RaycastResult result in hits) {
				if (result.gameObject.TryGetComponent(out InventorySlot slot)
						&& !slot.ParentInventory.Equals(ParentSlot.ParentInventory)
							&& !slot.Occupied) {
					//grabbedFrom.RemoveItem();
					//slot.FillSlot(grabbedItem);

					slot.ParentInventory.TryEnterItem(this, slot);
				}
			}

			transform.SetParent(ParentSlot.transform);
			selected = false;
		}

		public void FillSlot (InventorySlot slot) {
			ParentSlot = slot;
		}

		public override ItemObject GetLinked () {
			return Item;
		}
	}
}