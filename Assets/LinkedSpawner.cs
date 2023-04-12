using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Events;
using TankGame.Units.Interactions;

namespace TankGame.UI {

	//This is stinky global class meant to hack the UI here working until we have more of the UI systems figured out
    public class LinkedSpawner : MonoBehaviour {

		private void Start () {
			EventBus.AddListener<InteractionEvent<GenericInteraction>>(SearchingListener);
		}

		private void SearchingListener (InteractionEvent<GenericInteraction> _event) {
			if (_event.Name.Equals("Search")) {

			}
		}
	}
}