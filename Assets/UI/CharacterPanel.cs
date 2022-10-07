using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPanel : MonoBehaviour, ISelection {

	public GameObject pagePrefab;

	private List<CharacterPage> pages = new List<CharacterPage>();

	private void Start() {
		List<Character> characters = Player.GetCharacters();

		for (int i = 0; i < characters.Count; i++) {
			pages.Add(CreatePage(characters[i]));
		}
	}

	private CharacterPage CreatePage (Character character) {
		GameObject obj = Instantiate(pagePrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
		RectTransform rectTransform = obj.GetComponent<RectTransform>();
		rectTransform.SetParent(transform);
		rectTransform.anchorMin = new Vector2(0.5f, 0.15f + (0.3f * pages.Count));
		rectTransform.anchorMax = new Vector2(0.5f, 0.15f + (0.3f * pages.Count));
		rectTransform.anchoredPosition = Vector2.zero;
		CharacterPage page = obj.GetComponent<CharacterPage>();
		page.LoadCharacter(character);

		return page;
	}

	public void OnSelect(ISelectable selectable) {
		if (selectable.GetType() != typeof(Character)) return;

		Character character = (Character)selectable;

		foreach (CharacterPage page in pages) {
			if (page.BoundCharacter.Equals(character)) {
				page.OnSelect();
				return;
			}
		}
	}
}