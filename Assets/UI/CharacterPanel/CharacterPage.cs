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

	public class CharacterPage : LinkedElement<Character> {

		[SerializeField]
		private Text nameText;

		public Character BoundCharacter { get; private set; }

		private Image background;

		public CharacterInventory CharacterInv { get; private set; }

		public override Character GetLinked () {
			return BoundCharacter;
		}

		protected override void Awake() {
			base.Awake();

			nameText = GetComponentInChildren<Text>();
			background = GetComponent<Image>();
			CharacterInv = GetComponentInChildren<CharacterInventory>();
		}

		public void LoadCharacter(Character character) {
			BoundCharacter = character;
			nameText.text = BoundCharacter.name;

			if (character.TryGetComponent(out AbstractInventory inv) && inv.GetType() == typeof(PersonalInventory)) {
				CharacterInv.Initialize((PersonalInventory)inv);
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
	}
}