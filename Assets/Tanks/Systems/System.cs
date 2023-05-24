using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Tanks.Systems {

	public class System : MonoBehaviour, IInteractable {

		[SerializeField]
		public int Health { get; private set; } = 100;

		protected Source manager;

		protected virtual void Awake () {
			manager = GetComponent<Source>();
		}

		public virtual List<AbstractInteractionFactory> GetInteractions() {
			return new List<AbstractInteractionFactory>();
		}

		public GameObject GetObject() {
			return gameObject;
		}

		public Source GetManager () {
			return manager;
		}
	}
}