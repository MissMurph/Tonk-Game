using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using TankGame.Items;
using TankGame.Tanks;
using TankGame.Units;
using TankGame.Units.Commands;
using TankGame.Units.Interactions;
using UnityEngine;
using static TankGame.Items.AbstractInventory;

namespace TankGame.UI {

	public class StoragePanel : InventoryElement {

		private Dictionary<Vector2Int, ItemIcon> iconMap = new Dictionary<Vector2Int, ItemIcon>();

		[SerializeField]
		private TankInventory linkedInv;

		private StorageSpace[,] storageSpaces;

		[SerializeField]
		private GameObject storageSpacePrefab;

		private RectTransform rTransform;

		[SerializeField]
		private const int cellSize = 32;

		protected override void Awake() {
			base.Awake();

			storageSpaces = new StorageSpace[linkedInv.Size.x, linkedInv.Size.y];
			rTransform = GetComponent<RectTransform>();

			rTransform.anchoredPosition = new Vector2(linkedInv.Size.x * cellSize / 2, 0);
		}

		private void Start() {
			EventBus.AddListener<InteractionEvent<InvInteraction>>(EnterItem);
			EventBus.AddListener<InteractionEvent<InvInteraction>>(TakeItem);
			EventBus.AddListener<InteractionEvent<TankInventory.TankInvInteraction>>(TankEnterItem);

			for (int x = 0; x < linkedInv.Size.x; x++) {
				for (int y = 0; y < linkedInv.Size.y; y++) {
					GameObject obj = Instantiate(storageSpacePrefab, transform);
					StorageSpace space = obj.GetComponent<StorageSpace>();

					//obj.transform.localPosition = 

					RectTransform rect = obj.GetComponent<RectTransform>();
					rect.anchoredPosition = new Vector3((x * cellSize) + cellSize/2, (y * cellSize) + cellSize/2, 0);

					storageSpaces[x, y] = space;
					space.Position = new Vector2Int(x, y);
				}
			}
		}
		

		public void DrawerOpen () {
			rTransform.anchoredPosition = new Vector2(-linkedInv.Size.x * cellSize / 2, 0);
		}

		public void DrawerClose () {
			rTransform.anchoredPosition = new Vector2(linkedInv.Size.x * cellSize / 2, 0);
		}

		public bool CanAcceptItem (ItemIcon item, Vector2Int pos) {
			ItemObject itemObj = item.Item;

			Vector2Int truePos = linkedInv.GetItemPos(item.Item);

			if (truePos.x != pos.x || truePos.y != pos.y) {
				Debug.LogWarning("Item's True Position doesn't match specific icon's position! Check where position data is coming from!");
				return false;
			}

			for (int x = 1; x < itemObj.Item.Size.x; x++) {
				for (int y = 11; y < itemObj.Item.Size.y; y++) {
					if (storageSpaces[pos.x + x, pos.y + y].Occupied) {
						Debug.LogWarning("Something else occupies requested space in UI!");
						return false;
					}
				}
			}

			return true;
		}

		public override bool TryEnterItem (ItemIcon item, InventorySlot slot) {
			if (item.ParentSlot.ParentInventory.GetLinked().TryGetComponent(out Character character)) {
				AbstractInteraction interaction = linkedInv.TryEnterItemAtPos(item.Item, character, ((StorageSpace)slot).Position);
				if (interaction != null) character.ExecuteCommand(new Interact(interaction));
				return true;
			}

			return false;
		}

		private void TankEnterItem (InteractionEvent<TankInventory.TankInvInteraction> _event) {
			if (_event.Interaction.Name.Equals("EnterItem") && ReferenceEquals(_event.Interaction.Parent, linkedInv) && _event.Phase.Equals(IPhase.Post) && _event.Result.Equals(IResult.Success)) {
				ItemObject item = _event.Interaction.Item;
				ItemIcon icon = (ItemIcon)LinkedCache.FindLinkedElement(item);
				Vector2Int pos = _event.Interaction.Position;

				for (int x = 1; x < item.Item.Size.x; x++) {
					for (int y = 1; y < item.Item.Size.y; y++) {
						storageSpaces[pos.x + x, pos.y + y].OccupyingItem = icon;
					}
				}

				iconMap.Add(pos, icon);

				icon.ParentSlot.ParentInventory.TryTakeItem(icon);
				icon.FillSlot(storageSpaces[pos.x, pos.y]);

				icon.transform.SetParent(transform);
				icon.transform.SetAsLastSibling();

				icon.RectTransform.anchorMin = new Vector2(((float)pos.x) / linkedInv.Size.x, ((float)pos.y) / linkedInv.Size.y);
				icon.RectTransform.anchorMax = new Vector2((((float)pos.x) + item.Item.Size.x) / linkedInv.Size.x, (((float)pos.y) + item.Item.Size.y) / linkedInv.Size.y);
				icon.RectTransform.sizeDelta = Vector2.zero;
				icon.RectTransform.localPosition = Vector3.zero;
			}
		}

		private void EnterItem (InteractionEvent<InvInteraction> _event) {
			if (_event.Interaction.Name.Equals("EnterItem") && ReferenceEquals(_event.Interaction.Parent, linkedInv) && _event.Phase.Equals(IPhase.Post) && _event.Result.Equals(IResult.Success)) {
				ItemObject item = _event.Interaction.Item;
				ItemIcon icon = (ItemIcon)LinkedCache.FindLinkedElement(item);

				Vector2Int pos = linkedInv.GetItemPos(item);

				if (pos.x < 0 || pos.y < 0) {
					Debug.LogWarning("No position with item found! Verify item exists");
					return;
				}

				for (int x = 1; x < item.Item.Size.x; x++) {
					for (int y = 1; y < item.Item.Size.y; y++) {
						storageSpaces[pos.x + x, pos.y + y].OccupyingItem = icon;
					}
				}

				iconMap.Add(new Vector2Int(pos.x, pos.y), icon);

				icon.ParentSlot.ParentInventory.TryTakeItem(icon);
				icon.FillSlot(storageSpaces[pos.x, pos.y]);

				icon.transform.SetParent(transform);
				icon.transform.SetAsLastSibling();

				icon.RectTransform.anchorMin = new Vector2(pos.x / linkedInv.Size.x, pos.y / linkedInv.Size.y);
				icon.RectTransform.anchorMin = new Vector2(pos.x + item.Item.Size.x / linkedInv.Size.x, pos.y + item.Item.Size.y / linkedInv.Size.y);
				icon.RectTransform.anchoredPosition = Vector2.zero;
			}
		}

		private void TakeItem (InteractionEvent<InvInteraction> _event) {
			if (_event.Interaction.Name.Equals("TakeItem") && ReferenceEquals(_event.Interaction.Parent, linkedInv) && _event.Phase.Equals(IPhase.Post) && _event.Result.Equals(IResult.Success)) {
				foreach (KeyValuePair<Vector2Int, ItemIcon> entry in iconMap) {
					if (ReferenceEquals(entry.Value.Item, _event.Interaction.Item)) {
						TryTakeItem(entry.Value);
						return;
					}
				}
			}
		}

		public override bool TryTakeItem (ItemIcon item) {
			foreach (KeyValuePair<Vector2Int, ItemIcon> entry in iconMap) {
				if (ReferenceEquals(entry.Value, item)) {
					for (int x = 1; x < item.Item.Item.Size.x; x++) {
						for (int y = 1; y < item.Item.Item.Size.y; y++) {
							storageSpaces[entry.Key.x + x, entry.Key.y + y].OccupyingItem = null;
						}
					}

					iconMap.Remove(entry.Key);
					return true;
				}
			}

			return false;
		}

		public override AbstractInventory GetLinked () {
			return linkedInv;
		}
	}
}