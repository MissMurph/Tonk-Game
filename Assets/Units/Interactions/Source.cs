using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Events;
using TankGame.Units.Ai;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Units.Interactions {

	public class Source : MonoBehaviour {

		private Dictionary<IInteractable, List<AbstractInteractionFactory>> interactablesMap = new Dictionary<IInteractable, List<AbstractInteractionFactory>>();
		private Dictionary<string, AbstractInteractionFactory> interactionsMap = new Dictionary<string, AbstractInteractionFactory>();

		private Dictionary<string, UnityEventBase> listenerMap = new Dictionary<string, UnityEventBase>();

		private List<Transform> inRangeTransforms = new List<Transform>();

		private Dictionary<string, PreRequisite> addedPreRequisites = new Dictionary<string, PreRequisite>();

		private Dictionary<Type, Dictionary<string, AbstractInteractionFactory>> typedFactories;

		protected virtual void Awake () {
			IInteractable[] interactables = GetComponents<IInteractable>();
			Scraper interactionScraper = new Scraper();

			foreach (IInteractable interactable in interactables) {
				interactable.Collect(interactionScraper);
			}

			typedFactories = interactionScraper.Submitted;
		}

		public void MakeRequest<T> (Actor actor, string name, Action<InteractionResult<T>> callback) {
			Type interactionType = typeof(T);

			if (typedFactories.TryGetValue(interactionType, out Dictionary<string, AbstractInteractionFactory> foundDict) 
				&& foundDict.TryGetValue(name, out AbstractInteractionFactory foundFac)) {
				callback(new InteractionResult<T>());
			}
		}

		public List<PreRequisite> GetPreRequisites () {
			List<PreRequisite> output = new List<PreRequisite>();

			output.AddRange(addedPreRequisites.Values);

			return output;
		}

		public void SubmitPreRequisite (string key, IEvaluator condition, State solution){
			PreRequisite output = new PreRequisite {
				condition = condition,
				solution = solution
			};

			addedPreRequisites.TryAdd(key, output);
		}

		public void ExpirePreRequisite (string key) {
			if (addedPreRequisites.ContainsKey(key)) {
				addedPreRequisites.Remove(key);
			}
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

	public struct InteractionResult<T> {
		public T Interaction { get; private set; }
		public Source Source { get; private set; }
		public Actor Actor { get; private set; }
		public IPhase Phase { get; private set; }
		public IResult Result { get; private set; }

		public InteractionResult(T _interaction, Source _source, Actor _actor, IPhase _phase, IResult _result) {
			Interaction = _interaction;
			Source = _source;
			Actor = _actor;
			Phase = _phase;
			Result = _result;
		}
	}

	public class Scraper {
		public Dictionary<Type, Dictionary<string, AbstractInteractionFactory>> Submitted { get; private set; } 

		public Scraper () {
			Submitted = new Dictionary<Type, Dictionary<string, AbstractInteractionFactory>>();
		}

		public void Submit (Type key, AbstractInteractionFactory factory) {
			if (Submitted.TryGetValue(key, out Dictionary<string, AbstractInteractionFactory> dict)) {
				dict.Add(factory.Name, factory);
			}
			else {
				Dictionary<string, AbstractInteractionFactory> newDict = new Dictionary<string, AbstractInteractionFactory>();
				newDict.Add(factory.Name, factory);
				Submitted.Add(key, newDict);
			}
		}
	}
}