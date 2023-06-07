using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Entities {

	public class EntityCache : MonoBehaviour {

		private Dictionary<Transform, Entity> registered;

		private void Awake () {
			registered = new Dictionary<Transform, Entity>();
		}
	}
}