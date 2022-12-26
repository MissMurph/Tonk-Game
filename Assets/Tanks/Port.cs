using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Tanks {

	public class Port : MonoBehaviour, IInteractable {

		private Tank parentTank;

		private void Awake() {
			parentTank = GetComponentInParent<Tank>();
		}

		public List<AbstractInteractionFactory> GetInteractions() {
			return new List<AbstractInteractionFactory>() { 
				new GenericInteractionFactory("Embark", TryUsePort), 
				new GenericInteractionFactory("Disembark", TryUsePort) 
			};
		}

		public GameObject GetObject() {
			return gameObject;
		}

		private InteractionContext<GenericInteraction> UsePort (GenericInteraction interaction) {
			Character character = interaction.ActingCharacter;

			if (!character.Embarked) {
				character.transform.SetParent(parentTank.transform);
				character.transform.localPosition = transform.localPosition;
				return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Success);
			}
			else {
				character.transform.SetParent(null);
				character.transform.position = transform.localPosition;
				return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Success);
			}
		}

		public GenericInteraction TryUsePort (Character character, string name) {
			return new GenericInteraction(UsePort, character, name, this);
		}
	}
}