using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Capabilities;
using TankGame.Events;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Units.Interactions {

	public class Actor : MonoBehaviour {

		private Dictionary<string, UnityEventBase> registeredListeners;

		private List<Transform> inRangeTransforms = new List<Transform>();

		private Character character;

		private void Awake () {
			registeredListeners = new Dictionary<string, UnityEventBase>();

			character = GetComponent<Character>();

			IInteractable[] found = GetComponents<IInteractable>();

			Collector listenCollector = new Collector(gameObject, ISide.Actor);

			foreach (IInteractable component in found) {
				component.OnCollection(listenCollector);
			}

			registeredListeners = listenCollector.Listeners;
		}

		public void MakeRequest (Source interactable, string key, UnityAction<Interactionlet> callback, params ICapability[] data) {
			interactable.Request(new Request { Callback = Callback, Actor = this, Key = key, Data = data });
		}

		public void Act (Source interactable, Interactionlet packet, UnityAction<Interactionlet> callback, params ICapability[] data) {
			interactable.Act(packet, this, Callback);
		}

		private void Callback (Interactionlet result) {
			if (registeredListeners.TryGetValue(result.Name, out UnityEventBase _event)) {
				UnityEvent<Interactionlet> typed = _event as UnityEvent<Interactionlet>;

				typed.Invoke(result);
			}

			InteractionEvent globalEvent = Post(result);
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

		private InteractionEvent Post (Interactionlet result) {
			InteractionEvent _event = new InteractionEvent(result, character, ISide.Actor);
			EventBus.Post(_event);
			return _event;
		}

		public struct Request {
			public Action<Interactionlet> Callback { get; internal set; }
			public Actor Actor { get; internal set; }
			public string Key { get; internal set; }
			public ICapability[] Data { get; internal set; }
		}
	}
}