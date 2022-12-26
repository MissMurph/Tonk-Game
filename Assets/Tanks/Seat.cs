using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Tanks.Stations {

	public class Seat : MonoBehaviour, IInteractable {

		public bool Occupied {
			get {
				return occupant != null;
			}
		}

		protected Character occupant;

		public bool Embark (Character character) {
			if (Occupied) return false;

			occupant = character;

			return true;
		}

		public void Disembark () {
			if (occupant == null) return;

			occupant = null;
		}

		public GameObject GetObject() {
			return gameObject;
		}

		public List<AbstractInteractionFactory> GetInteractions() {
			throw new System.NotImplementedException();
		}
	}
}