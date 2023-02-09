using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using UnityEngine;
using UnityEngine.Events;
using static TankGame.Items.AbstractInventory;

namespace TankGame.Units.Interactions {

	public class InteractionManager : MonoBehaviour {

		private Dictionary<IInteractable, List<AbstractInteractionFactory>> interactablesMap = new Dictionary<IInteractable, List<AbstractInteractionFactory>>();
		private Dictionary<string, AbstractInteractionFactory> interactionsMap = new Dictionary<string, AbstractInteractionFactory>();

		private Dictionary<string, UnityEventBase> listenerMap = new Dictionary<string, UnityEventBase>();

		private List<Transform> inRangeTransforms = new List<Transform>();

		private void Awake () {
			IInteractable[] interactables = GetComponents<IInteractable>();

			foreach (IInteractable interactable in interactables) {
				List<AbstractInteractionFactory> intFactories = interactable.GetInteractions();
				interactablesMap.Add(interactable, intFactories);

				foreach (AbstractInteractionFactory factory in intFactories) {
					interactionsMap.Add(factory.Name, factory);
				}
			}
		}

		public InteractionContext Interact (AbstractInteraction interaction) {
			return interaction.Act(this);
		}

		public void AddListener<T> (string name, UnityAction<InteractionContext<T>> listener) where T : AbstractInteraction<T> {
			if (listenerMap.TryGetValue(name, out UnityEventBase evnt) && evnt.GetType().Equals(typeof(UnityEvent<InteractionContext<T>>))) {
				UnityEvent<InteractionContext<T>> superType = (UnityEvent<InteractionContext<T>>)evnt;

				superType.AddListener(listener);
			}
			else {
				UnityEvent<InteractionContext<T>> unityEvent = new UnityEvent<InteractionContext<T>>();
				unityEvent.AddListener(listener);
				listenerMap.Add(name, unityEvent);
			}
		}

		public InteractionContext<T> Post<T> (InteractionContext<T> context) where T : AbstractInteraction<T> {
			if (listenerMap.TryGetValue(context.Name, out UnityEventBase baseEvent) && baseEvent.GetType().Equals(typeof(UnityEvent<InteractionContext<T>>))) {
				UnityEvent<InteractionContext<T>> evnt = (UnityEvent<InteractionContext<T>>)baseEvent;

				evnt.Invoke(context);
			}

			EventBus.Post(new InteractionEvent<T>(context));
			return context;
		}

		public AbstractInteraction RequestInteraction<T> (string name, T target, Character character) where T : AbstractInteraction<T> {
			if (interactionsMap.TryGetValue(name, out AbstractInteractionFactory factory) && factory.TargetType.Equals(typeof(T))) {
				AbstractInteraction interaction = factory.GetAsType<T>().Construct(target, character);

				if (interaction != null) return interaction;
			}

			Debug.LogWarning("No interaction created! Null value provided");
			return null;
		}

		//This one will only request the default target for the interaction, if none exists then this will never work
		public AbstractInteraction RequestInteraction (string name, Character character) {
			if (interactionsMap.TryGetValue(name, out AbstractInteractionFactory factory)) {
				AbstractInteraction interaction = factory.Construct(character);

				if (interaction != null) return interaction;
			}

			Debug.LogWarning("No interaction created! Null value provided");
			return null;
		}

		//This is bad stinky temp method that will be replaced when this whole system stops being spaghet
		//All this does is loop through each factory and tries to construct an interaction
		//Need to better define default interactions
		public AbstractInteraction RequestInteraction (Character character) {
			foreach (KeyValuePair<string, AbstractInteractionFactory> entry in interactionsMap) {
				AbstractInteraction test = entry.Value.Construct(character);

				if (test == null) continue;

				return test;
			}

			Debug.LogWarning("No interaction created! Null value provided");
			return null;
		}

		//interaction trigger
		private void OnTriggerEnter2D (Collider2D collision) {
			Transform parentTransform = collision.transform.root;

			inRangeTransforms.Add(collision.transform);
		}

		private void OnTriggerExit2D (Collider2D collision) {
			if (inRangeTransforms.Contains(collision.transform)) {
				inRangeTransforms.Remove(collision.transform);
			}
		}

		public bool IsInRange (Transform transform) {
			return inRangeTransforms.Contains(transform);
		}

		public List<Transform> TransformsInRange () {
			return new List<Transform>(inRangeTransforms);
		}
	}
}