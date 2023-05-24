using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Ai;
using TankGame.Units.Interactions;
using TankGame.Units.Navigation;
using UnityEngine;

namespace TankGame.Tanks {

	public class Seat : SerializedMonoBehaviour, IInteractable, ITraversable {

		[SerializeField]
		private ITraversable parent;

		public bool Occupied {
			get {
				return Occupant != null;
			}
		}

		[ReadOnly]
		[OdinSerialize]
		public Character Occupant { get; private set; }

		private Source manager;

		private void Awake() {
			if (parent is null) parent = transform.parent.TryGetComponent(out ITraversable traversable) ? traversable : null;
			manager = GetComponent<Source>();

			foreach (Character character in GetComponentsInChildren<Character>()) {
				character.Traversable = this;
				Occupant = character;
				character.StateMachine.SubmitPreRequisite("seated", new NeedsToDisembark(this), new Interacting(TrySit, "Unsit"));
			}
		}

		public List<AbstractInteractionFactory> GetInteractions() {
			return new List<AbstractInteractionFactory>() {
				new GenericInteractionFactory("Sit", TrySit),
				new GenericInteractionFactory("Unsit", TrySit)
			};
		}

		public GameObject GetObject() {
			return gameObject;
		}

		private InteractionContext<GenericInteraction> Sit(GenericInteraction interaction) {
			Character character = interaction.ActingCharacter;

			if (interaction.Name == "Sit" && !ReferenceEquals(character.Traversable, this)) {
				character.transform.SetParent(transform);
				character.transform.localPosition = Vector3.zero;
				character.Traversable = this;
				Occupant = character;
				character.StateMachine.SubmitPreRequisite("seated", new NeedsToDisembark(this), new Interacting(TrySit, "Unsit"));
				return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Continue);
			}
			else if (interaction.Name == "Unsit") {
				character.transform.SetParent(parent.GetObject().transform);
				character.transform.localPosition = transform.localPosition;
				character.Traversable = parent;
				Occupant = null;
				character.StateMachine.ExpirePreRequisite("seated");
				return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Success);
			}

			//Both If statements catch all actual attempts to use this, executions reaching here are continuations
			return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Continue);
		}

		public GenericInteraction TrySit(Character character, string name) {
			return new GenericInteraction(Sit, character, name, this);
		}

		public void FindPath(PathRequest request, Action<PathResult> callback) {
			//what happens if we do nothing lol
			//NOTHING GOOD

			Vector3[] outPut = new Vector3[1] { Vector3.zero };
			callback(new PathResult(outPut, true, request.callback));
		}

		public Source GetManager() {
			return manager;
		}
	}
}