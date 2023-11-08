using System.Collections.Generic;
using TankGame.Events;
using TankGame.Players;
using TankGame.Units;
using UnityEngine;

namespace TankGame.UI {

	public class CharacterPanel : MonoBehaviour {

		public GameObject pagePrefab;

		private List<CharacterPage> pages = new List<CharacterPage>();

		private void Start() {
			List<Character> characters = Player.GetCharacters();

			for (int i = 0; i < characters.Count; i++) {
				pages.Add(CreatePage(characters[i]));
			}

			EventBus.Subscribe<PlayerEvent.Selection>(OnSelect);
			//EventBus.AddListener<InventoryEvent.ItemTransfered>(OnItemTransfered);
		}

		private CharacterPage CreatePage(Character character) {
			GameObject obj = Instantiate(pagePrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
			RectTransform rectTransform = obj.GetComponent<RectTransform>();
			rectTransform.SetParent(transform);
			rectTransform.anchorMin = new Vector2(0.5f, 0.2f + (0.4f * pages.Count));
			rectTransform.anchorMax = new Vector2(0.5f, 0.2f + (0.4f * pages.Count));
			rectTransform.anchoredPosition = Vector2.zero;
			CharacterPage page = obj.GetComponent<CharacterPage>();
			page.LoadCharacter(character);

			return page;
		}

		private void OnSelect(PlayerEvent.Selection _event) {

			Character character = _event.Selectable as Character;

			if (character != null) {
				foreach (CharacterPage page in pages) {
					if (page.BoundCharacter.Equals(character)) {
						page.OnSelect(_event.SelectStatus);
						return;
					}
				}
			}
		}

		/*private void OnItemTransfered(InventoryEvent.ItemTransfered _event) {
			if (_event.Inventory.GetObject().TryGetComponent(out Character character)) {
				CharacterPage page = GetPage(character);

				ItemIcon icon = page.GetItemIcon(_event.Item);

				if (icon != null) {
					CharacterPage targetPage = GetPage(_event.Target.GetObject().GetComponent<Character>());

					for (int i = 0; i < targetPage.slotIcons.Length; i++) {
						if (!targetPage.slotIcons[i].Occupied) {
							if (icon.ParentSlot != null) icon.ParentSlot.RemoveItem();
							targetPage.slotIcons[i].FillSlot(icon);
							break;
						}
					}
				}
			}
		}*/

		private CharacterPage GetPage (Character character) {
			foreach (CharacterPage page in pages) {
				if (page.BoundCharacter.Equals(character)) return page;
			}

			return null;
		}
	}
}