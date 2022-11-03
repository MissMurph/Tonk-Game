using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Players;

namespace TankGame.UI {

	public class ItemIcon : MonoBehaviour {

		public ItemObject Item { get; private set; }

		private InventorySlot parentSlot;

		private bool selected = false;

		private RectTransform rectTransform;

		public InventorySlot ParentSlot {
			get {
				return parentSlot;
			}
		}

		[SerializeField]
		private Image image;

		private void Awake() {
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

		public void Initialize(ItemObject _parentItem) {
			Item = _parentItem;
			image.sprite = Item.Item.Icon;
		}

		public void OnClick () {
			selected = true;
		}

		public void OnRelease () {
			selected = false;
		}

		private void TransferCallback () {

		}
	}
}