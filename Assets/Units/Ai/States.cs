using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TankGame.Units.Ai {

	public class States : SerializedMonoBehaviour {

		private delegate State StateConstructor(string name);

		private static States instance;

		[SerializeField]
		private Dictionary<string, State> registry = new Dictionary<string, State>();

		//public static State IDLE_STANDING = Register("idle_standing", (name) => new IdleStanding(name));
		//public static State IDLE_WANDERING = Register("idle_wandering", (name) => new IdleWandering(name));

		private void Awake () {
			instance = this;
		}

		private static State Register (string name, StateConstructor constructor) {
			State state = constructor.Invoke(name);
			instance.registry.TryAdd(name, state);
			return state;
		}

		private void OnDestroy () {
			instance = null;
		}
	}
}