using System.Collections;
using System.Collections.Generic;
using TankGame.Items;
using TankGame.Units;
using TankGame.Players;
//using TankGame.Player.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Events;

namespace TankGame.UI {

	public class CharacterPage : MonoBehaviour {

		[SerializeField]
		private Text nameText;

		public Character BoundCharacter { get; private set; }

		private Image background;

		[SerializeField]
		private GameObject invSlotPrefab;

		[SerializeField]
		private GameObject itemIconPrefab;

		[SerializeField]
		private GameObject inventorySpace;

		public InventorySlot[] slotIcons;

		private void Awake() {
			nameText = GetComponentInChildren<Text>();
			background = GetComponent<Image>();
		}

		public void LoadCharacter(Character character) {
			BoundCharacter = character;
			nameText.text = BoundCharacter.name;

			if (character.TryGetComponent(out IInventory inv) && inv.GetType() == typeof(PersonalInventory)) {
				PersonalInventory pInv = (PersonalInventory)inv;

				slotIcons = new InventorySlot[pInv.slotCount];

				for (int i = 0; i < pInv.slotCount; i++) {
					GameObject obj = Instantiate(invSlotPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
					RectTransform rectTransform = obj.GetComponent<RectTransform>();
					InventorySlot slotComp = obj.GetComponent<InventorySlot>();


					float x = 0.25f;
					float y = 0.75f;

					if (i == 1 || i == 3) x += 0.5f;
					if (i > 1) y -= 0.5f;

					Vector2 anchor = new Vector2(x, y);

					rectTransform.SetParent(inventorySpace.transform);
					rectTransform.anchorMin = anchor;
					rectTransform.anchorMax = anchor;
					rectTransform.anchoredPosition = Vector2.zero;

					ItemObject occupyingItem = pInv.GetAtSlot(i);

					if (occupyingItem != null) {
						//slotIcons[i].sprite = occupyingItem.Item.Icon;
						ItemIcon itemIconObj = Instantiate(itemIconPrefab, obj.transform).GetComponent<ItemIcon>();
						//RectTransform iconRect = obj.GetComponent<RectTransform>();
						//iconRect.position = Vector2.zero;
						itemIconObj.Initialize(occupyingItem);
						slotComp.Initialize(pInv, i, itemIconObj);
					}
					else slotComp.Initialize(pInv, i);

					slotIcons[i] = slotComp;
				}
			}
		}

		private static List<Transform> GetChildren(Transform parent) {
			List<Transform> list = new List<Transform>();

			foreach (Transform child in parent) {
				list.Add(child);
			}

			return list;
		}

		public void OnClick() {
			bool status = Player.Controller.Select(BoundCharacter);

			background.color = status ? Color.cyan : Color.white;
		}

		public void OnSelect(bool status) {
			background.color = status ? Color.cyan : Color.white;
		}

		public ItemIcon GetItemIcon (ItemObject item) {
			for (int i = 0; i < slotIcons.Length; i++) {
				if (slotIcons[i].occupyingItem != null && slotIcons[i].occupyingItem.Item.Equals(item)) {
					return slotIcons[i].occupyingItem;
				}
			}

			return null;
		}
	}
}