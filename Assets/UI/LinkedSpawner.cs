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
			if (_event.Interaction.Name.Equals("Search") && _event.Interaction.Parent is SupplyCache) {
				SupplyCache cacheInv = _event.Interaction.Parent as SupplyCache;
				LinkedElement<SupplyCache> cacheElement = LinkedCache.FindLinkedElement(cacheInv);

				//If the element already exists, we do not want to instantiate
				if (cacheElement is null) {
					GameObject cacheElementObj = Instantiate(UIPrefabs.WorldPrefabs.CacheElement, transform);
					ItemCache cacheComponent = cacheElementObj.GetComponent<ItemCache>();
					cacheComponent.Initialize(cacheInv);
				}
			}
		}
	}
}