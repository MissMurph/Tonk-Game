using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Events;
using TankGame.Units.Interactions;
using TankGame.Items;

namespace TankGame.UI {

	//This is stinky global class meant to hack the UI here working until we have more of the UI systems figured out
    public class LinkedSpawner : MonoBehaviour {

		private void Start () {
			EventBus.Subscribe<InteractionEvent<GenericInteraction>>(SearchingListener);
		}

		//When we detect a Search Interaction being performed, we'll check the Linked Cache if the corresponding UI Element exists
		private void SearchingListener (InteractionEvent<GenericInteraction> _event) {
			if (_event.Name.Equals("Search")) {
				//if (LinkedCache.FindLinkedElement<SupplyCache>(_event.)) ;
			}
		}
	}
}