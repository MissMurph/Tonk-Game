using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Commands : MonoBehaviour {

		private static Dictionary<string, CommandFactory> commands = new Dictionary<string, CommandFactory>();

		public static readonly CommandFactory Move = RegisterCommand<Move, Vector2>("move", (target) => new Move(target));
		public static readonly CommandFactory Interact = RegisterCommand<Interact, IInteractable>("interact", (target) => new Interact(target));
		//public static readonly CommandFactory TransferItem = RegisterCommand<TransferItemCommand, IInventory>("transfer_item", (target) => new TransferItemCommand(target));

		private void Start() {

		}

		public static C Construct<C, T>(CommandFactory factory, T value) where C : Command<T> {
			if (commands.ContainsValue(factory)) {
				CommandFactory<C, T> newFactory = factory.GetAsType<C, T>();
				return newFactory.Construct(value);
			}

			else return null;
		}

		public static C Construct<C, T>(string name, T value) where C : Command<T> {
			if (commands.TryGetValue(name, out CommandFactory factory)) {
				return Construct<C, T>(factory, value);
			}

			else return null;
		}

		//T is the Command type, which will be used to make the relevant CommandFactory that will output the Command as the specified class T.
		private static CommandFactory RegisterCommand<C, T>(string name, CommandFactory<C, T>.CommConstructor constructor) {
			CommandFactory factory = new CommandFactory<C, T>(name, constructor);
			commands.Add(name, factory);
			return factory;
		}

		public class CommandFactory<C, T> : CommandFactory {

			public delegate C CommConstructor(T target);

			private CommConstructor constructor;

			public CommandFactory(string name, CommConstructor constructor) : base(name, typeof(C)) {
				this.constructor = constructor;
			}

			public C Construct(T _target) {
				return constructor.Invoke(_target);
			}
		}

		public class CommandFactory {

			public string Name { get; private set; }
			public Type CommandType { get; private set; }

			internal CommandFactory<C, T> GetAsType<C, T>() {
				if (typeof(C) == CommandType) {
					return (CommandFactory<C, T>)this;
				}

				else return null;
			}

			internal CommandFactory(string _name, Type _commandType) {
				Name = _name;
				CommandType = _commandType;
			}
		}
	}
}