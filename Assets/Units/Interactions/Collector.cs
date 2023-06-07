using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankGame.Units.Interactions {

	public class Collector {

		public Dictionary<string, Interaction> Interactions { get; private set; }

		public Dictionary<string, UnityEventBase> Listeners { get; private set; }

		private GameObject parent;

		public ISide Side { get; private set; }

		public Collector (GameObject _parent, ISide _side) {
			Interactions = new Dictionary<string, Interaction>();
			parent = _parent;
			Side = _side;
		}

		public Interaction Submit (string key, Interaction.InteractionDel _destination, Interaction.InteractionDel _constructor, IInteractable _parent) {
			if (Interactions.TryGetValue(key, out Interaction found)) {
				Debug.LogError(key + " interaction already submitted to " + parent.name + "!");
				return null;
			}
			else {
				Interaction factory = new Interaction(_destination, _constructor, key, _parent);
				Interactions.Add(key, factory);
				return factory;
			}
		}

		public void AttachListener (string key, UnityAction<Interactionlet> listener) {
			UnityEvent<Interactionlet> _event = Listeners.GetValueOrDefault(key, new UnityEvent<Interactionlet>()) as UnityEvent<Interactionlet>;

			_event.AddListener(listener);
		}
	}
}