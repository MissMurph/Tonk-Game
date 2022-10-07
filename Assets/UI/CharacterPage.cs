using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPage : MonoBehaviour {

	[SerializeField]
	private Text nameText;
	public Character BoundCharacter { get; private set; }
	private Image background;

	private void Awake() {
		nameText = GetComponentInChildren<Text>();
		background = GetComponent<Image>();
	}

	public void LoadCharacter(Character character) {
		BoundCharacter = character;
		nameText.text = BoundCharacter.name;
	}

	public void OnClick () {
		bool status = Player.Select(BoundCharacter);

		background.color = status ? Color.cyan : Color.white;
	}

	public void OnSelect() {

	}
}