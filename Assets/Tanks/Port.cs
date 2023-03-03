using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;
using TankGame.Units.Navigation;
using TankGame.Units.Ai;

namespace TankGame.Tanks {

	public class Port : MonoBehaviour, IInteractable {

		private Tank parentTank;

		private InteractionManager manager;

		private void Awake() {
			parentTank = GetComponentInParent<Tank>();
			manager = GetComponent<InteractionManager>();
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

			if (!ReferenceEquals(character.Traversable, parentTank)) {
				character.transform.SetParent(parentTank.transform);
				character.transform.localPosition = transform.localPosition;
				character.Traversable = parentTank;
				character.StateMachine.SubmitPreRequisite("embarked", new NeedsToDisembark(parentTank), new Interacting(TryUsePort, "Disembark"));
				return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Success);
			}
			else {
				character.transform.SetParent(World.GlobalTraversable.GetObject().transform);
				character.transform.localPosition = transform.localPosition;
				character.Traversable = World.GlobalTraversable;
				character.StateMachine.ExpirePreRequisite("embarked");
				return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Success);
			}
		}

		public GenericInteraction TryUsePort (Character character, string name) {
			return new GenericInteraction(UsePort, character, name, this);
		}

		public InteractionManager GetManager() {
			return manager;
		}
	}
}