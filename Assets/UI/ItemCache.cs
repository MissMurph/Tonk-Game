using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using TankGame.Items;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEditor.PackageManager;
using UnityEngine;

namespace TankGame.UI {

	public class ItemCache : InventoryElement {

		private SupplyCache linkedInv;

		private Dictionary<int, InventorySlot> slots = new Dictionary<int, InventorySlot>();

		private ProgressBar searchProgress;

		protected override void Awake () {
			base.Awake();

			EventBus.Subscribe<InteractionEvent<GenericInteraction>>(OnSearchComplete);
		}

		private void Update () {
			Vector3 worldPos = linkedInv.transform.position;
			worldPos.y += 2;

			Vector2 uiPos = Camera.main.WorldToScreenPoint(worldPos);

			UITransform.position = uiPos;
		}

		public void Initialize (SupplyCache link) {
			linkedInv = link;

			int totalSize = linkedInv.slotCount * 50;
			int halfway = totalSize / 2;

			UITransform.sizeDelta = new Vector2(totalSize, 50);

			GameObject progressBar = Instantiate(UIPrefabs.ProgressBar, transform);
			searchProgress = progressBar.GetComponent<ProgressBar>();
			searchProgress.Set(0);
			searchProgress.gameObject.SetActive(false);

			for (int i = 0; i < linkedInv.slotCount; i++) {
				GameObject slot = Instantiate(UIPrefabs.WorldPrefabs.CacheElement, transform);

				RectTransform slotTransform = slot.GetComponent<RectTransform>();

				int x = ((i * 50) + 5) - halfway;

				slotTransform.localPosition = new Vector3(x, 0, 0);

				slots.Add(i, slot.GetComponent<InventorySlot>());
				if (linkedInv.SearchProgress < 1) slot.SetActive(false);
			}
		}

		private void OnSearchComplete (InteractionEvent<GenericInteraction> _event) {
			if (ReferenceEquals(_event.Interaction.Parent.GetObject(), linkedInv.gameObject)) {
				//We need to reveal the progress bar when starting to search
				if (_event.Result.Equals(IResult.Start)) {
					searchProgress.gameObject.SetActive(true);
				}

				//Every time it triggers we need to update the progress bar
				if (_event.Result.Equals(IResult.Continue)) {
					searchProgress.Set(linkedInv.SearchProgress * 100);
				}

				if (_event.Result.Equals(IResult.Success)) {
					foreach (InventorySlot slot in slots.Values) {
						slot.gameObject.SetActive(true);
					}

					searchProgress.gameObject.SetActive(false);
					//We can unsubscribe from the event once we know searching is complete
					EventBus.Unsubscribe<InteractionEvent<GenericInteraction>>(OnSearchComplete);
				}
			}
		}

		public override AbstractInventory GetLinked () {
			return linkedInv;
		}

		public override bool TryEnterItem (ItemIcon item, InventorySlot slot) {
			if (!slot.Occupied && item.ParentSlot.ParentInventory.GetLinked().TryGetComponent(out Character character)) {
				AbstractInteraction interaction = linkedInv.TryEnterItemUI(item.Item, character);
			}


			return false;
		}

		public override bool TryTakeItem (ItemIcon item) {
			throw new System.NotImplementedException();
		}
	}
}