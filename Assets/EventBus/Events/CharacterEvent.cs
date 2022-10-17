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

		public class CommandEvent : CharacterEvent {
			public Command.CommandContext Context { get; private set; }

			public CommandEvent(Command.CommandContext context) : base("command_" + context.Command.Name, context.Character) {
				Context = context;
			}
		}
	}
}