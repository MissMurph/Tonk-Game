using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPage : MonoBehaviour {

	[SerializeField]
	private Text nameText;

	private void Awake() {
		
	}

	public void LoadCharacter(Character character) {
		nameText.text = character.name;
	}
}