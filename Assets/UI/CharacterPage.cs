using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPage : MonoBehaviour {

	[SerializeField]
	private Text nameText;
	private Character boundCharacter;
	private Image background;

	private void Awake() {
		nameText = GetComponentInChildren<Text>();
		background = GetComponent<Image>();
	}

	public void LoadCharacter(Character character) {
		boundCharacter = character;
		nameText.text = boundCharacter.name;
	}

	public void OnClick () {
		bool status = Player.Select(boundCharacter);

		background.color = status ? Color.cyan : Color.white;
	}
}