using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Tanks.Systems {

	public class System : MonoBehaviour {

		[SerializeField]
		public int Health { get; private set; } = 100;
	}
}