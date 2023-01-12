using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class IdleWandering : State {
		public override string Name { get; protected set; }
		public override Transform Target { get; protected set; }

		private int waitInterval = 10;	//-1 4 times per second, 4 = 1 second

		public IdleWandering (string _name) {
			Name = _name;
		}

		public IdleWandering () {

		}

		public override void Act (Character actor) {
			waitInterval--;

			if (waitInterval <= 0) {
				Vector3 pos = actor.transform.position;

				System.Random rando = new System.Random();

				pos += new Vector3(rando.Next(-5, 5), rando.Next(-5, 5));

				actor.SubmitTarget(pos, (yeet) => { });

				waitInterval = rando.Next(8, 32);
			}
		}
	}
}