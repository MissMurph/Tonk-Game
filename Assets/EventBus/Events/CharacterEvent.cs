using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Units;
using TankGame.Units.Commands;

namespace TankGame.Events {

	public class CharacterEvent : AbstractEvent {

		public Character Character { get; private set; }

		protected CharacterEvent(string name, Character character) : base(name) {
			Character = character;
		}
	}
}