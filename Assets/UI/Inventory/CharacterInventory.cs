using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using TankGame.Items;
using TankGame.UI;
using TankGame.Units;
using TankGame.Units.Commands;
using TankGame.Units.Interactions;
using UnityEngine;
using static TankGame.Items.AbstractInventory;

namespace TankGame.UI {

	public class CharacterInventory : InventoryElement {

		private PersonalInventory linkedInv;

		private InventorySlot[] slots;

		public override AbstractInventory GetLinked () {
			return linkedInv;
		}

		public void Initialize (PersonalInventory inv) {
			linkedInv = inv;

			slots = new InventorySlot[inv.slotCount];

			for (int i = 0; i < inv.slotCount; i++) {
				GameObject slot = Instantiate(UIPrefabs.CharacterPrefabs.InventorySlot, Vector3.zero, Quaternion.Euler(Vector3.zero), this.transform);
				RectTransform rectTransform = slot.GetComponent<RectTransform>();
				InventorySlot slotComp = slot.GetComponent<InventorySlot>();

				float x = 0.25f;
				float y = 0.75f;

				if (i == 1 || i == 3) x += 0.5f;
				if (i > 1) y -= 0.5f;

				Vector2 anchor = new Vector2(x, y);

				rectTransform.anchorMin = anchor;
				rectTransform.anchorMax = anchor;
				rectTransform.anchoredPosition = Vector2.zero;

				ItemObject occupyingItem = inv.GetAtSlot(i);

				if (occupyingItem != null) {
					//slots[i].sprite = occupyingItem.Item.Icon;
					ItemIcon itemIconObj = Instantiate(UIPrefabs.CharacterPrefabs.ItemIcon, slot.transform).GetComponent<ItemIcon>();
					//RectTransform iconRect = obj.GetComponent<RectTransform>();
					//iconRect.position = Vector2.zero;
					itemIconObj.Initialize(occupyingItem, slotComp);
					slotComp.OccupyingItem = itemIconObj;
				}

				slots[i] = slotComp;
			}

			EventBus.AddListener<InteractionEvent<InvInteraction>>(EnterItem);
			EventBus.AddListener<InteractionEvent<InvInteraction>>(TakeItem);
		}

		private void EnterItem (InteractionEvent<InvInteraction> _event) {
			if (_event.Interaction.Name.Equals("EnterItem")) {
				if (ReferenceEquals(_event.Interaction.Parent, linkedInv) && _event.Phase.Equals(IPhase.Post) && _event.Result.Equals(IResult.Success)) {
					ItemIcon itemIcon = (ItemIcon)LinkedCache.FindLinkedElement(_event.Interaction.Item);

					if (itemIcon == null) {
						Debug.LogWarning("Could not find linked ItemIcon!");
						return;
					}

					EnterItemUnsafe(itemIcon, slots[linkedInv.GetItemIndex(_event.Interaction.Item)]);
				}
			}
		}

		private void TakeItem (InteractionEvent<InvInteraction> _event) {
			if (_event.Interaction.Name.Equals("TakeItem")) {
				if (ReferenceEquals(_event.Interaction.Parent, linkedInv) && _event.Phase.Equals(IPhase.Post) && _event.Result.Equals(IResult.Success)) {
					foreach (InventorySlot slot in slots) {
						if (ReferenceEquals(slot.OccupyingItem.Item, _event.Interaction.Item)) {
							slot.OccupyingItem = null;
							return;
						}
					}
				}

				if (ReferenceEquals(_event.Interaction.ActingCharacter, linkedInv.GetComponent<Character>()) && _event.Phase.Equals(IPhase.Post) && _event.Result.Equals(IResult.Success)) {
					int index = linkedInv.GetItemIndex(_event.Interaction.Item);

					if (index == -1) {
						Debug.LogWarning("No index found for item by UI!");
						return;
					}

					ItemIcon itemIcon = (ItemIcon)LinkedCache.FindLinkedElement(_event.Interaction.Item);

					EnterItemUnsafe(itemIcon, slots[index]);
				}
			}
		}

		private void EnterItemUnsafe (ItemIcon icon, InventorySlot slot) {
			icon.ParentSlot.ParentInventory.TryTakeItem(icon);
			icon.FillSlot(slot);
			slot.OccupyingItem = icon;

			icon.transform.SetParent(slot.transform);
			icon.transform.SetAsLastSibling();

			icon.RectTransform.anchoredPosition = Vector2.zero;
		}

		public override bool TryEnterItem (ItemIcon item, InventorySlot slot) {
			if (item.ParentSlot.ParentInventory.GetLinked().TryGetComponent(out Character character)) {
				AbstractInteraction interaction = linkedInv.TryEnterItemUI(item.Item, character);
				if (interaction != null) character.CommManager.ExecuteCommand(new Interact(interaction));
				return true;
			}
			else {
				Character linkedChar = linkedInv.GetComponent<Character>();
				AbstractInteraction interaction = item.ParentSlot.ParentInventory.GetLinked().TryTakeItemUI(item.Item, linkedChar);
				if (interaction != null) linkedChar.CommManager.ExecuteCommand(new Interact(interaction));
				return true;
			}
		}

		public override bool TryTakeItem (ItemIcon item) {
			for (int i = 0; i < slots.Length; i++) {
				if (ReferenceEquals(slots[i].OccupyingItem, item)) {
					slots[i].OccupyingItem = null;

					return true;
				}
			}

			Debug.LogWarning("Item not found in CharacterInventory to remove!");
			return false;
		}
	}
}