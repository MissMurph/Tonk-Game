using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPanel : MonoBehaviour {

	public GameObject pagePrefab;

	private LinkedList<CharacterPage> pages;

	private void Start() {
		List<Character> characters = Player.Instance.GetCharacters();

		for (int i = 0; i < characters.Count; i++) {
			
		}
	}

	private GameObject CreatePage (Character character) {
		GameObject obj = Instantiate(pagePrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
		CharacterPage page = obj.GetComponent<CharacterPage>();
		page.LoadCharacter(character);

		return obj;
	}
}